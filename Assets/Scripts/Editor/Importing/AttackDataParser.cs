using SummonsTracker.Characters;
using SummonsTracker.EditorUtilities;
using SummonsTracker.Rolling;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Importer
{
    public static class AttackDataParser
    {
        public static bool TryMakeAttack(SerializedProperty actionsSerializedProperty, int index, string title, string body, out AttackData attackData)
        {
            var split = body.Split(new[] { "<em>", "</em>" }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length < 4)
            {
                attackData = null;
                return false;
            }

            if (!GetAttackType(split[0], out var attackType))
            {
                attackData = null;
                return false;
            }
            if (!GetHitInfo(split[1], out var attackMod, out var range, out var maxRange, out var target))
            {
                attackData = null;
                return false;
            }

            var normalNumber = 0;
            var normalFaces = 0;
            var normalModifier = 0;
            var normalDamage = DamageTypes.none;
            var plusNumber = 0;
            var plusFaces = 0;
            var plusModifier = 0;
            var plusDamage = DamageTypes.none;

            var hasDoubleDamage = false;

            const string plus = "plus";
            var plusIndex = split[3].ToLower().IndexOf(plus);
            if (plusIndex == -1)
            {
                if (!GetDamage(split[3], out normalNumber, out normalFaces, out normalModifier, out normalDamage))
                {
                    attackData = null;
                    return false;
                }
            }
            else
            {
                var pre = split[3].Substring(0, plusIndex);
                if (!GetDamage(pre, out normalNumber, out normalFaces, out normalModifier, out normalDamage))
                {
                    attackData = null;
                    return false;
                }
                var pst = split[3].Substring(plusIndex + plus.Length);
                if (GetDamage(pst, out plusNumber, out plusFaces, out plusModifier, out plusDamage))
                {
                    hasDoubleDamage = true;
                }

            }
            var remaining = string.Join("\n", split.Skip(4).Select(s => s.Trim()).ToArray());
            if (string.IsNullOrEmpty(remaining))
            {
                remaining = split.Last();
                var dotText = remaining.IndexOf('.');
                if (dotText != -1 && remaining.Substring(0, dotText).Trim().ToLower().EndsWith("damage"))
                {
                    remaining = remaining.Substring(dotText + 1).Trim();
                }
            }

            var hasSavingThrow = false;
            if (SavingThrowParser.TryMakeSavingThrow(title, GetFromDC(remaining), out var savingThrow, out var remainingFromSavingThrow))
            {
                hasSavingThrow = true;
            }
            else if (SavingThrowParser.TryMakeSavingThrow(title, remaining, out savingThrow, out remainingFromSavingThrow))
            {
                hasSavingThrow = true;
            }

            attackData = hasSavingThrow && hasDoubleDamage ? ScriptableObject.CreateInstance<DoubleDamageSavingThrowData>()
                : hasSavingThrow && !hasDoubleDamage ? ScriptableObject.CreateInstance<AttackAndSavingThrowData>()
                : !hasSavingThrow && hasDoubleDamage ? ScriptableObject.CreateInstance<AttackSecondDamageData>()
                : ScriptableObject.CreateInstance<AttackData>();

            using (var serializedObject = new SerializedObject(attackData))
            {
                using (var atkProperty = serializedObject.FindProperty("_attackType"))
                {
                    SerializedPropertyHelper.AssignEnumObj(atkProperty, attackType);
                }
                using (var atkModProperty = serializedObject.FindProperty("_attackMod"))
                {
                    atkModProperty.intValue = attackMod;
                }
                using (var rangeProperty = serializedObject.FindProperty("_range"))
                {
                    rangeProperty.intValue = range;
                }
                using (var maxRangeProperty = serializedObject.FindProperty("_maxRange"))
                {
                    maxRangeProperty.intValue = maxRange;
                }
                using (var targetProperty = serializedObject.FindProperty("_target"))
                {
                    targetProperty.stringValue = target;
                }
                using (var damageDiceProperty = serializedObject.FindProperty("_damage"))
                {
                    using var numberProperty = damageDiceProperty.FindPropertyRelative("_number");
                    using var facesProperty = damageDiceProperty.FindPropertyRelative("_faces");
                    using var modifiersProperty = damageDiceProperty.FindPropertyRelative("_modifiers");

                    numberProperty.intValue = normalNumber;
                    facesProperty.intValue = normalFaces;
                    modifiersProperty.intValue = normalModifier;
                }
                using (var damageProperty = serializedObject.FindProperty("_damageType"))
                {
                    SerializedPropertyHelper.AssignEnumObj(damageProperty, normalDamage);
                }
                if (hasSavingThrow)
                {
                    SavingThrowParser.AssignSavingThrow(serializedObject, savingThrow);
                    using (var noteProperty = serializedObject.FindProperty("_note"))
                    {
                        noteProperty.stringValue = ActionDataParser.GetNote(remainingFromSavingThrow);
                    }
                }
                else
                {
                    using (var noteProperty = serializedObject.FindProperty("_note"))
                    {
                        noteProperty.stringValue = ActionDataParser.GetNote(remaining);
                    }
                }
                if (hasDoubleDamage)
                {
                    using (var damageDiceProperty = serializedObject.FindProperty("_secondaryDamage"))
                    {
                        using var numberProperty = damageDiceProperty.FindPropertyRelative("_number");
                        using var facesProperty = damageDiceProperty.FindPropertyRelative("_faces");
                        using var modifiersProperty = damageDiceProperty.FindPropertyRelative("_modifiers");

                        numberProperty.intValue = plusNumber;
                        facesProperty.intValue = plusFaces;
                        modifiersProperty.intValue = plusModifier;
                    }
                    using (var damageProperty = serializedObject.FindProperty("_secondaryDamageType"))
                    {
                        SerializedPropertyHelper.AssignEnumObj(damageProperty, plusDamage);
                    }
                }
                serializedObject.ApplyModifiedProperties();
            }
            return true;
        }

        private static bool GetAttackType(string text, out AttackType type)
        {
            var attackTypeKey = text.Replace(" ", "").ToLower();
            if (attackTypeKey.EndsWith(":"))
            {
                attackTypeKey = attackTypeKey.Substring(0, attackTypeKey.Length - 1);
            }
            var attackTypes = Enum.GetValues(typeof(AttackType));
            foreach (var atk in attackTypes)
            {
                if (attackTypeKey == atk.ToString().ToLower())
                {
                    type = (AttackType)atk;
                    return true;
                }
            }
            type = default;
            return false;
        }

        private static bool GetHitInfo(string text, out int attackMod, out int range, out int maxRange, out string target)
        {
            //		text	" +9 to hit, reach 10 ft. one target. "	string
            var t = text.Trim();
            attackMod = 0;
            range = 0;
            maxRange = 0;
            target = string.Empty;

            var attackModStr = t.Trim().ToLower();
            const string toHitSuffix = "to hit";
            var toHitSuffixIndex = attackModStr.IndexOf(toHitSuffix);
            if (toHitSuffixIndex == -1)
            {
                return false;
            }
            attackModStr = attackModStr.Substring(0, toHitSuffixIndex).Trim().TrimStart('+').Trim();
            if (!int.TryParse(attackModStr, out attackMod))
            {
                return false;
            }

            var reachStr = t.Trim();
            const string ftSuffix = "ft";
            var reachPrefixIndex = GetIndex(reachStr, out var reachPrefix);
            var ftSuffixIndex = reachStr.IndexOf(ftSuffix);

            if (reachPrefixIndex == -1)
            {
                reachPrefixIndex = 0;
            }
            if (ftSuffixIndex == -1)
            {
                ftSuffixIndex = reachStr.Length - 1;
            }
            reachStr = reachStr.Substring(reachPrefixIndex + reachPrefix.Length, ftSuffixIndex - reachPrefixIndex - reachPrefix.Length).Trim();

            if (!TryGetRange(reachStr, out range, out maxRange))
            {
                var reachSplit = reachStr.Split(' ');
                var reachFound = false;
                for (int i = 0; i < reachSplit.Length; i++)
                {
                    if (TryGetRange(reachSplit[i], out range, out maxRange))
                    {
                        reachFound = true;
                        break;
                    }
                }
                if (!reachFound)
                {
                    return false;
                }
            }

            var targetsString = new[] { "target", "creature" };
            for (int i = 0; i < targetsString.Length; i++)
            {
                var targetIndex = t.IndexOf(targetsString[i]);
                if (targetIndex != -1)
                {
                    target = t.Substring(ftSuffixIndex + ftSuffix.Length, targetIndex - ftSuffixIndex - ftSuffix.Length + targetsString[i].Length);
                    break;
                }
            }
            if (string.IsNullOrEmpty(target))
            {
                target = t.Substring(ftSuffixIndex + ftSuffix.Length);
            }
            target = target.Trim(' ', '.', ',', ',');
            return true;
        }

        private static bool TryGetRange(string rangeString, out int range, out int maxRange)
        {
            if (int.TryParse(rangeString, out range))
            {
                maxRange = 0;
                return true;
            }
            else
            {
                var reachSplit = rangeString.Split('/');
                if (reachSplit.Length < 2)
                {
                    range = 0;
                    maxRange = 0;
                    return false;
                }
                if (!int.TryParse(reachSplit[0].Trim(), out range))
                {
                    range = 0;
                    maxRange = 0;
                    return false;
                }
                if (!int.TryParse(reachSplit[1].Trim(), out maxRange))
                {
                    range = 0;
                    maxRange = 0;
                    return false;
                }
                return true;
            }
        }

        private static int GetIndex(string reachString, out string prefix)
        {
            var prefixes = new[] { "range", "reach" };
            for (int i = 0; i < prefixes.Length; i++)
            {
                var index = reachString.IndexOf(prefixes[i]);
                if (index != -1)
                {
                    prefix = prefixes[i];
                    return index;
                }
            }
            prefix = string.Empty;
            return 0;
        }

        private static bool GetDamage(string text, out int number, out int faces, out int modifier, out DamageTypes damageType)
        {
            var bOpenInd = text.IndexOf('(');
            var bCloseInd = text.IndexOf(')');

            if (bOpenInd < 0 || bCloseInd < 0)
            {
                var damageTypes = Enum.GetValues(typeof(DamageTypes));
                foreach (DamageTypes dmg in damageTypes)
                {
                    var damageTypeIndex = text.ToLower().IndexOf(dmg.ToString().ToLower());
                    if (damageTypeIndex != -1)
                    {
                        var dice = text.Substring(0, damageTypeIndex);
                        DiceUtility.FromString(dice, out number, out faces, out modifier);
                        damageType = dmg;
                        return true;
                    }
                }
                number = 0;
                faces = 0;
                modifier = 0;
                damageType = DamageTypes.none;
                return false;
            }
            else
            {
                var dice = text.Substring(bOpenInd + 1, bCloseInd - bOpenInd - 1);
                DiceUtility.FromString(dice, out number, out faces, out modifier);
                var remaining = text.Substring(bCloseInd + 1).Trim();

                var damageTypes = Enum.GetValues(typeof(DamageTypes));
                foreach (var dmg in damageTypes)
                {
                    var dmgStr = dmg.ToString().ToLower();
                    if (remaining.StartsWith(dmgStr))
                    {
                        damageType = (DamageTypes)dmg;
                        return true;
                    }
                }
                damageType = DamageTypes.none;
                return false;
            }
        }

        private static string GetFromDC(string remaining)
        {
            var dcIndex = remaining.IndexOf("DC");
            if (dcIndex != -1)
            {
                remaining = remaining.Substring(dcIndex);
            }
            return remaining;
        }

    }
}