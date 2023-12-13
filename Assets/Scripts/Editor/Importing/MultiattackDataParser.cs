using SummonsTracker.Characters;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Importer
{
    public static class MultiattackDataParser
    {
        public static bool TryMakeMultiattack(SerializedProperty actionsSerializedProperty, int index, string title, string body, out MultiattackData multiattackData)
        {
            if (title.ToLower().Contains("multiattack"))
            {
                var inst = ScriptableObject.CreateInstance<MultiattackData>();
                using (var serializedObject = new SerializedObject(inst))
                {
                    using (var noteProperty = serializedObject.FindProperty("_note"))
                    {
                        noteProperty.stringValue = body;
                    }
                    serializedObject.ApplyModifiedProperties();
                }
                multiattackData = inst;
                return true;
            }
            multiattackData = null;
            return false;
        }

        public static void FillMultiattack(MultiattackData multiattackData, string characterName, SerializedProperty actionsSerializedProperty)
        {
            using var serializedObject = new SerializedObject(multiattackData);
            using var attacksProperty = serializedObject.FindProperty("_attacks");
            var note = multiattackData.Note;
            var characterIndex = note.ToLower().IndexOf(characterName.ToLower());
            if (characterIndex != -1)
            {
                note = note.Substring(characterIndex + characterName.Length).Trim();
            }
            var atLeast1 = false;
            for (int i = 0; i < actionsSerializedProperty.arraySize; i++)
            {
                using (var serializedProperty = actionsSerializedProperty.GetArrayElementAtIndex(i))
                {
                    if (serializedProperty.objectReferenceValue is MultiattackData)
                    {
                        continue;
                    }
                    var atkName = serializedProperty.objectReferenceValue.name;
                    var attack = note;
                    var attackNameIndex = attack.ToLower().IndexOf(atkName.ToLower());
                    if (attackNameIndex != -1)
                    {
                        var addedThisAttack = false;
                        attack = attack.Substring(0, attackNameIndex).Trim();
                        int maxIndex = 0;
                        for (int j = 0; j < actionsSerializedProperty.arraySize; j++)
                        {
                            if (i == j)
                            {
                                continue;
                            }
                            using (var otherSerializedProperty = actionsSerializedProperty.GetArrayElementAtIndex(j))
                            {
                                var otherName = otherSerializedProperty.objectReferenceValue.name.ToLower();
                                var otherAttackNameIndex = attack.ToLower().IndexOf(otherName);

                                if (otherAttackNameIndex != -1)
                                {
                                    maxIndex = Mathf.Max(maxIndex, otherAttackNameIndex + otherName.Length);
                                }
                            }
                        }
                        attack = attack.Substring(maxIndex).Trim();
                        if (!string.IsNullOrEmpty(attack))
                        {
                            var split = attack.Split(' ');

                            for (int j = split.Length - 1; j >= 0; j--)
                            {
                                if (ParseNumber(split[j].ToLower(), out var number))
                                {
                                    var length = attacksProperty.arraySize;
                                    attacksProperty.arraySize = length + 1;
                                    using var attackProperty = attacksProperty.GetArrayElementAtIndex(length);
                                    using var attackIndexProperty = attackProperty.FindPropertyRelative("_attackIndex");
                                    using var numberProperty = attackProperty.FindPropertyRelative("_attackNumber");

                                    attackIndexProperty.intValue = i;
                                    numberProperty.intValue = number;
                                    atLeast1 = true;
                                    addedThisAttack = true;
                                    break;
                                }
                            }
                        }
                        if (!addedThisAttack)
                        {
                            var length = attacksProperty.arraySize;
                            attacksProperty.arraySize = length + 1;
                            using var attackProperty = attacksProperty.GetArrayElementAtIndex(length);
                            using var attackIndexProperty = attackProperty.FindPropertyRelative("_attackIndex");
                            using var numberProperty = attackProperty.FindPropertyRelative("_attackNumber");
                            attackIndexProperty.intValue = i;
                            numberProperty.intValue = 1;
                        }
                    }
                }
            }
            if (!atLeast1)
            {
                var remaining = multiattackData.Note;
                var attackIndex = remaining.ToLower().IndexOf("attack");
                if (attackIndex != -1)
                {
                    remaining = remaining.Substring(0, attackIndex);
                }
                var split = remaining.Split(' ');

                for (int i = split.Length - 1; i >= 0; i--)
                {
                    if (ParseNumber(split[i].ToLower(), out var number))
                    {
                        var length = attacksProperty.arraySize;
                        attacksProperty.arraySize = length + 1;
                        using var attackProperty = attacksProperty.GetArrayElementAtIndex(length);
                        using var attackIndexProperty = attackProperty.FindPropertyRelative("_attackIndex");
                        using var numberProperty = attackProperty.FindPropertyRelative("_attackNumber");
                        attackIndexProperty.intValue = -1;
                        numberProperty.intValue = number;
                        break;
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private static bool ParseNumber(string number, out int result)
        {
            result = number switch
            {
                "one" => 1,
                "two" => 2,
                "three" => 3,
                "four" => 4,
                "five" => 5,
                "six" => 6,
                "seven" => 7,
                "eight" => 8,
                "nine" => 9,
                "ten" => 10,
                _ => int.TryParse(number, out var i) ? i : -1
            };
            return result != -1;
        }
    }
}