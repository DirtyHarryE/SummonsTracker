using SummonsTracker.Characters;
using SummonsTracker.EditorUtilities;
using SummonsTracker.Rolling;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Importer
{
    public static class SavingThrowParser
    {
        public static bool TryMakeSavingThrow(SerializedProperty actionsSerializedProperty, int index, string title, string body, out SavingThrowData savingThrowData)
        {
            if (TryMakeSavingThrow(title, body, out var savingThrow, out var remaining))
            {
                var inst = ScriptableObject.CreateInstance<SavingThrowData>();
                using (var serializedObject = new SerializedObject(inst))
                {
                    AssignSavingThrow(serializedObject, savingThrow);

                    using (var noteProperty = serializedObject.FindProperty("_note"))
                    {
                        noteProperty.stringValue = remaining;
                    }
                    serializedObject.ApplyModifiedProperties();
                }
                savingThrowData = inst;
                return true;
            }
            savingThrowData = null;
            return false;
        }

        public static bool TryMakeSavingThrow(string title, string body, out ISavingThrow savingThrow, out string remainingText)
        {
            var t = body.Trim();

            const string dcString = "DC";
            var dcIndex = t.IndexOf(dcString);

            var dc = 0;
            StatType type = StatType.none;
            var grapple = false;

            const string savingThrowString = "saving throw";
            var savingThrowIndex = t.ToLower().IndexOf(savingThrowString);

            if (savingThrowIndex != -1 && dcIndex != -1)
            {
                var savingThrowText = t.Substring(dcIndex + dcString.Length, savingThrowIndex - dcIndex - dcString.Length).Trim();

                var dcValueIndex = savingThrowText.IndexOf(' ');
                if (dcValueIndex == -1)
                {
                    savingThrow = null;
                    remainingText = body;
                    return false;
                }
                var dcStringVal = savingThrowText.Substring(0, dcValueIndex).Trim();

                if (!int.TryParse(dcStringVal, out dc))
                {
                    savingThrow = null;
                    remainingText = body;
                    return false;
                }

                var typeString = savingThrowText.Substring(dcValueIndex + 1).Trim();
                var split = typeString.Split(new string[] { ",", "or" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < split.Length; i++)
                {
                    if (Enum.TryParse(split[i].Trim(), out StatType thistype))
                    {
                        type |= thistype;
                    }
                }
            }
            if (savingThrowIndex == -1 && dcIndex != -1)
            {
                int grappleIndex = t.ToLower().IndexOf("grapple");
                if (grappleIndex != -1)
                {
                    var escapeStr = "escape";
                    int escapeIndex = t.ToLower().IndexOf(escapeStr);
                    if (escapeIndex != -1)
                    {
                        var dcStr = t.Substring(dcIndex + dcString.Length).Trim();
                        var split = dcStr.Split(' ');
                        for (int i = 0; i < split.Length; i++)
                        {
                            if (int.TryParse(split[i].Trim('(', ')', ' '), out var newDC))
                            {
                                dc = newDC;
                                grapple = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        savingThrow = null;
                        remainingText = body;
                        return false;
                    }

                }
                else
                {
                    savingThrow = null;
                    remainingText = body;
                    return false;
                }
            }
            else if (savingThrowIndex == -1 || dcIndex == -1)
            {
                savingThrow = null;
                remainingText = body;
                return false;
            }

            if (type == StatType.none && !grapple)
            {
                savingThrow = null;
                remainingText = body;
                return false;
            }
            var consequence = t.Substring(savingThrowIndex + savingThrowString.Length).Trim();


            FailSaveOutcome fail;
            SuccessSaveOutcome succ;
            if (ExtractConsequence(consequence, out var failCondition, out var failDice, out var failDamage, out var half, out var failOther, out var removeOtherFromNote))
            {
                fail = new FailSaveOutcome(failCondition == ConditionTypes.none
                                     ? string.IsNullOrEmpty(failOther)
                                        ? failDamage == DamageTypes.none
                                            ? FailSavingThrowOutcomes.Nothing
                                            : FailSavingThrowOutcomes.Damage
                                        : failDamage == DamageTypes.none
                                            ? FailSavingThrowOutcomes.Other
                                            : FailSavingThrowOutcomes.Damage
                                     : failDamage == DamageTypes.none
                                        ? FailSavingThrowOutcomes.Condition
                                        : FailSavingThrowOutcomes.DamageAndCondition,

                                     failDice, failDamage,
                                     failCondition, failOther);
                succ = new SuccessSaveOutcome(half ? SuccessSavingThrowOutcomes.Half : SuccessSavingThrowOutcomes.Nothing,
                    Dice.None, DamageTypes.none,
                    ConditionTypes.none, "");

            }
            else
            {
                fail = new FailSaveOutcome(FailSavingThrowOutcomes.Nothing, Dice.None, DamageTypes.none, ConditionTypes.none, "");
                succ = new SuccessSaveOutcome(SuccessSavingThrowOutcomes.Nothing, Dice.None, DamageTypes.none, ConditionTypes.none, "");
            }
            savingThrow = grapple ? new SavingThrow(title, true, dc, fail, succ) : new SavingThrow(title, type, dc, fail, succ);

            if (grapple)
            {
                remainingText = body;
                var bOpenInd = remainingText.IndexOf('(');
                var bCloseInd = remainingText.IndexOf(')');

                if (bOpenInd != -1 && bCloseInd != -1)
                {
                    remainingText = $"{remainingText.Substring(0, bOpenInd).Trim()} {remainingText.Substring(bCloseInd + 1).Trim()}";
                }
            }
            else
            {
                remainingText = body.Replace(t.Substring(dcIndex, savingThrowIndex - dcIndex), string.Empty).Replace("  ", " ");
            }
            if (removeOtherFromNote)
            {
                var otherIndex = remainingText.ToLower().IndexOf(failOther.ToLower());
                if (otherIndex != -1)
                {
                    var pre = remainingText.Substring(0, otherIndex);
                    var pst = remainingText.Substring(otherIndex + failOther.Length, remainingText.Length - otherIndex - failOther.Length);
                    remainingText = $"{pre}{pst}";
                }
            }
            if (failDamage != DamageTypes.none)
            {
                var takePrefixes = new[] { "taking", "take" };
                for (int i = 0; i < takePrefixes.Length; i++)
                {
                    var takeIndex = remainingText.IndexOf(takePrefixes[i]);
                    if (takeIndex != -1)
                    {
                        remainingText = remainingText.Substring(0, takeIndex);
                        break;
                    }
                }
            }
            remainingText = remainingText.Trim(' ', ',');
            if (remainingText.ToLower() != "saving throw")
            {
                if (!remainingText.EndsWith("."))
                {
                    remainingText = $"{remainingText}.";
                }
                remainingText = $"{char.ToUpper(remainingText[0])}{remainingText.Substring(1)}";
            }
            else
            {
                remainingText = string.Empty;
            }
            return true;
        }

        public static bool ExtractConsequence(string consequence, out ConditionTypes condition, out Dice damageDice, out DamageTypes damageType, out bool half, out string other, out bool removeOtherFromNote)
        {
            removeOtherFromNote = true;

            condition = ConditionTypes.none;
            damageDice = Dice.None;
            damageType = DamageTypes.none;
            half = false;
            other = string.Empty;

            const string damageSuffix = "damage";
            var takePrefixes = new[] { "taking", "take" };
            for (int i = 0; i < takePrefixes.Length; i++)
            {
                var takeIndex = consequence.IndexOf(takePrefixes[i]);
                if (takeIndex != -1)
                {
                    var damage = consequence.Substring(takeIndex + takePrefixes[i].Length).Trim();

                    var damageIndex = damage.ToLower().IndexOf(damageSuffix);
                    if (damageIndex == -1)
                    {
                        damageIndex = damage.Length - 1;
                    }
                    damage = damage.Substring(0, damageIndex).Trim();

                    var bOpenInd = damage.IndexOf('(');
                    var bCloseInd = damage.IndexOf(')');

                    if (bOpenInd != -1 && bCloseInd != -1)
                    {
                        var dice = damage.Substring(bOpenInd + 1, bCloseInd - bOpenInd - 1).Trim();
                        var damageTypeStr = damage.Substring(bCloseInd + 1).Trim();
                        DiceUtility.FromString(dice, out int number, out int faces, out int modifiers);

                        foreach (DamageTypes d in Enum.GetValues(typeof(DamageTypes)))
                        {
                            if (d.ToString().ToLower() == damageTypeStr.ToLower())
                            {
                                damageType = d;
                                damageDice = new Dice(number, faces, modifiers);

                                var newDamageIndex = consequence.ToLower().IndexOf(damageSuffix, takeIndex + takePrefixes[i].Length);
                                if (newDamageIndex != -1)
                                {
                                    var remainder = consequence.Substring(newDamageIndex + damageSuffix.Length).Trim();
                                    var failIndex = remainder.ToLower().IndexOf("fail");
                                    var successIndex = remainder.ToLower().IndexOf("success");
                                    if (failIndex != -1 && successIndex != -1)
                                    {
                                        var success = remainder.Substring(failIndex, successIndex - failIndex);
                                        if (success.ToLower().Contains("half"))
                                        {
                                            half = true;
                                        }
                                        else
                                        {
                                            var successDamageIndex = success.IndexOf(damageSuffix);
                                            if (successDamageIndex != -1)
                                            {

                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    break;
                }
            }

            var orPrefixes = new[] { "or", "and" };
            var sub = string.Empty;
            foreach (string orPrefix in orPrefixes.OrderByDescending(s => s.Length))
            {
                var orIndex = consequence.IndexOf(orPrefix);
                if (orIndex != -1)
                {
                    sub = consequence.Substring(orIndex + orPrefix.Length);
                    if (half && sub.ToLower().Contains("half"))
                    {
                        continue;
                    }
                    var dotIndex = sub.IndexOf('.');
                    if (dotIndex != -1)
                    {
                        sub = sub.Substring(0, dotIndex);
                    }
                    break;
                }
            }
            if (!string.IsNullOrEmpty(sub))
            {
                sub = sub.Trim();

                var conditions = Enum.GetValues(typeof(ConditionTypes));
                foreach (ConditionTypes c in conditions)
                {
                    if (sub.ToLower().Contains(c.ToString().ToLower()))
                    {
                        condition = c;
                        break;
                    }
                }
                if (condition == ConditionTypes.none)
                {
                    other = $"{char.ToUpper(sub[0])}{sub.Substring(1)}";
                    removeOtherFromNote = false;
                }
                else
                {
                    var untils = new[] { "until", "for" };
                    for (int i = 0; i < untils.Length; i++)
                    {
                        var untilIndex = consequence.IndexOf(untils[i]);
                        if (untilIndex != -1)
                        {
                            var until = consequence.Substring(untilIndex).Trim();
                            other = $"{char.ToUpper(until[0])}{until.Substring(1)}";
                            break;
                        }
                    }
                }
            }
            return true;
        }

        public static void AssignSavingThrow(SerializedObject serializedObject, ISavingThrow savingThrow,
            string dcPropertyName = "_dc", string grapplePropertyName = "_isGrapple", string savingThrowPropertyName = "_savingThrow", string failPropertyName = "_failOutcome", string successPropertyName = "_successOutcome")
        {
            using (var dcProperty = serializedObject.FindProperty(dcPropertyName))
            {
                dcProperty.intValue = savingThrow.DC;
            }
            if (savingThrow.IsGrapple)
            {
                using (var grappleProperty = serializedObject.FindProperty(grapplePropertyName))
                {
                    grappleProperty.boolValue = true;
                }
            }
            else
            {
                using (var savingThrowTypeProperty = serializedObject.FindProperty(savingThrowPropertyName))
                {
                    var enumNames = savingThrowTypeProperty.enumNames;
                    for (int i = 0; i < enumNames.Length; i++)
                    {
                        if (Enum.TryParse(enumNames[i], out StatType savingThrowType))
                        {
                            if ((savingThrowType & savingThrow.SavingThrow) != 0)
                            {
                                savingThrowTypeProperty.intValue |= (int)savingThrowType;
                            }
                        }
                    }
                }
            }

            using (var failProperty = serializedObject.FindProperty(failPropertyName))
            {
                using (var failType = failProperty.FindPropertyRelative("_failSaveType"))
                {
                    SerializedPropertyHelper.AssignEnumObj(failType, savingThrow.FailureSavingThrowOutcome.FailSaveType);
                }
                using (var damageProperty = failProperty.FindPropertyRelative("_damage"))
                {
                    using var numberProperty = damageProperty.FindPropertyRelative("_number");
                    using var facesProperty = damageProperty.FindPropertyRelative("_faces");
                    using var modifiersProperty = damageProperty.FindPropertyRelative("_modifiers");

                    var dice = savingThrow.FailureSavingThrowOutcome.Damage;
                    numberProperty.intValue = dice.Number;
                    facesProperty.intValue = dice.Faces;
                    modifiersProperty.intValue = dice.Modifiers;
                }
                using (var damageTypeProperty = failProperty.FindPropertyRelative("_damageType"))
                {
                    SerializedPropertyHelper.AssignEnumObj(damageTypeProperty, savingThrow.FailureSavingThrowOutcome.DamageType);
                }
                using (var conditionTypeProperty = failProperty.FindPropertyRelative("_condition"))
                {
                    SerializedPropertyHelper.AssignEnumObj(conditionTypeProperty, savingThrow.FailureSavingThrowOutcome.Condition);
                }
                using (var outcomeProperty = failProperty.FindPropertyRelative("_outcomeNote"))
                {
                    outcomeProperty.stringValue = savingThrow.FailureSavingThrowOutcome.OutcomeNote;
                }
            }
            using (var successProperty = serializedObject.FindProperty(successPropertyName))
            {
                using (var successType = successProperty.FindPropertyRelative("_successSaveType"))
                {
                    SerializedPropertyHelper.AssignEnumObj(successType, savingThrow.SuccessSavingThrowOutcome.SuccessSaveType);
                }
                using (var damageProperty = successProperty.FindPropertyRelative("_damage"))
                {
                    using var numberProperty = damageProperty.FindPropertyRelative("_number");
                    using var facesProperty = damageProperty.FindPropertyRelative("_faces");
                    using var modifiersProperty = damageProperty.FindPropertyRelative("_modifiers");

                    var dice = savingThrow.SuccessSavingThrowOutcome.Damage;
                    numberProperty.intValue = dice.Number;
                    facesProperty.intValue = dice.Faces;
                    modifiersProperty.intValue = dice.Modifiers;
                }
                using (var damageTypeProperty = successProperty.FindPropertyRelative("_damageType"))
                {
                    SerializedPropertyHelper.AssignEnumObj(damageTypeProperty, savingThrow.SuccessSavingThrowOutcome.DamageType);
                }
                using (var conditionTypeProperty = successProperty.FindPropertyRelative("_condition"))
                {
                    SerializedPropertyHelper.AssignEnumObj(conditionTypeProperty, savingThrow.SuccessSavingThrowOutcome.Condition);
                }
                using (var outcomeProperty = successProperty.FindPropertyRelative("_outcomeNote"))
                {
                    outcomeProperty.stringValue = savingThrow.SuccessSavingThrowOutcome.OutcomeNote;
                }
            }
        }

    }
}