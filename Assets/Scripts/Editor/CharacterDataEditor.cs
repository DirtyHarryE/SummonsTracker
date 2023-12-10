using SummonsTracker.Rolling;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SummonsTracker.Characters
{
    [CustomEditor(typeof(CharacterData))]
    public class CharacterDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var character = (CharacterData)target;
            EditorGUI.BeginChangeCheck();

            using (var nameSerializedProperty = serializedObject.FindProperty("_name"))
            {
                EditorGUILayout.PropertyField(nameSerializedProperty);
            }
            using (var creatureSerializedProperty = serializedObject.FindProperty("_creature"))
            {
                EditorGUILayout.PropertyField(creatureSerializedProperty);
            }

            DrawHorizontalLine();

            using (var acSerializedProperty = serializedObject.FindProperty("_ac"))
            {
                acSerializedProperty.intValue = EditorGUILayout.IntField("Armor Class", acSerializedProperty.intValue);
            }
            using (var hpSerializedProperty = serializedObject.FindProperty("_maxHP"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(hpSerializedProperty.displayName);

                    var rect = EditorGUILayout.GetControlRect();

                    using var numberProperty = hpSerializedProperty.FindPropertyRelative("_number");
                    using var facesProperty = hpSerializedProperty.FindPropertyRelative("_faces");
                    using var modifiersProperty = hpSerializedProperty.FindPropertyRelative("_modifiers");

                    var average = DiceUtility.Average(numberProperty.intValue, facesProperty.intValue, modifiersProperty.intValue);
                    var avgGUIContent = new GUIContent(average.ToString());

                    var width = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = Mathf.Max(20, EditorStyles.label.CalcSize(avgGUIContent).x);

                    DiceEditorUtility.DrawDice(rect, avgGUIContent, hpSerializedProperty);


                    EditorGUIUtility.labelWidth = width;
                }
            }
            DrawSpeed();

            DrawHorizontalLine();

            using (new EditorGUILayout.HorizontalScope())
            {
                var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 2f);
                var step = rect.width / 6f;
                DrawStat(new Rect(rect.x, rect.y, step, rect.height),
                    "STR", "_strength", character.StrengthMod);
                DrawStat(new Rect(rect.x + step, rect.y, step, rect.height),
                    "DEX", "_dexterity", character.DexterityMod);
                DrawStat(new Rect(rect.x + (step * 2), rect.y, step, rect.height),
                    "CON", "_constitution", character.ConstitutionMod);
                DrawStat(new Rect(rect.x + (step * 3), rect.y, step, rect.height),
                    "INT", "_intelligence", character.IntelligenceMod);
                DrawStat(new Rect(rect.x + (step * 4), rect.y, step, rect.height),
                    "WIS", "_wisdom", character.WisdomMod);
                DrawStat(new Rect(rect.x + (step * 5), rect.y, step, rect.height),
                    "CHA", "_charisma", character.CharismaMod);
            }

            DrawHorizontalLine();

            EnumFlagsHelper.DrawEnumFlags(serializedObject, "_savingThrows", getName: (index, name) =>
            {
                var stats = (StatType)index;
                var mod = character.GetStatMod(stats) + character.Proficiency;
                var modStr = mod >= 0 ? $"+{mod}" : mod.ToString();
                return $"{name} {modStr}";
            });
            var prev = (StatType)1;
            EnumFlagsHelper.DrawEnumFlags(serializedObject, "_skills", onAddMenu: (menu, property, index) =>
            {
                var skill = (Skills)index;
                var stat = SkillsUtility.GetStatType(skill);
                if (stat != prev)
                {
                    prev = stat;
                    menu.AddSeparator(string.Empty);
                }
            }, getName: (index, name) =>
            {
                var skills = (Skills)index;
                var stats = SkillsUtility.GetStatType(skills);
                var mod = character.GetStatMod(stats) + character.Proficiency;
                var modStr = mod >= 0 ? $"+{mod}" : mod.ToString();
                return $"{name} {modStr}";
            });
            EnumFlagsHelper.DrawEnumFlags(serializedObject, "_damageVulnerabilities", getName: (i, n) => n.ToLower());
            EnumFlagsHelper.DrawEnumFlags(serializedObject, "_damageResistances", getName: (i, n) => n.ToLower());
            EnumFlagsHelper.DrawEnumFlags(serializedObject, "_damageImmunities", getName: (i, n) => n.ToLower());
            EnumFlagsHelper.DrawEnumFlags(serializedObject, "_conditionImmunities", getName: (i, n) => n.ToLower());

            using (var profSerializedProperty = serializedObject.FindProperty("_proficiency"))
            {
                profSerializedProperty.intValue = EditorGUILayout.IntField("Proficiency Bonus", profSerializedProperty.intValue);
            }

            DrawHorizontalLine();

            if (_actionDataEditor == null)
            {
                _actionDataEditor = new ActionListDrawer(serializedObject, (CharacterData)target);
            }
            _actionDataEditor.UsePersistentData = EditorUtility.IsPersistent(target);
            _actionDataEditor.DrawLayout();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawStat(Rect position, string statName, string propName, int modifier)
        {
            var w = position.width * 0.5f;
            var h = position.height * 0.5f;
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, h),
                                 statName,
                                 new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });

            using (var serializedProperty = serializedObject.FindProperty(propName))
            {
                serializedProperty.intValue = EditorGUI.IntField(new Rect(position.x, position.y + h, w, h),
                                                                 serializedProperty.intValue,
                                                                 new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleCenter });
            }
            EditorGUI.LabelField(new Rect(position.x + w, position.y + h, w, h),
                $"({(modifier >= 0 ? $"+{modifier}" : modifier.ToString())})",
                new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
        }

        private void DrawHorizontalLine(int height = 3)
        {
            EditorGUILayout.Space(5f);
            var rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;

            EditorGUI.DrawRect(rect, _yellow);
            EditorGUILayout.Space(5f);
        }

        private void DrawSpeed()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Speed");
                using var movementSerializedProperty = serializedObject.FindProperty("_movement");
                int toDelete = -1;
                for (int i = 0; i < movementSerializedProperty.arraySize; i++)
                {
                    using var movementElementProperty = movementSerializedProperty.GetArrayElementAtIndex(i);
                    using var distanceProperty = movementElementProperty.FindPropertyRelative("Distance");
                    distanceProperty.intValue = EditorGUILayout.DelayedIntField(distanceProperty.intValue, GUILayout.Width(22));
                    EditorGUILayout.LabelField(i == 0 ? _feetLabel.text : $"{_feetLabel.text}, ", GUILayout.Width(GUI.skin.label.CalcSize(_feetLabel).x));
                    using var typeProperty = movementElementProperty.FindPropertyRelative("Type");
                    if (typeProperty.intValue != (int)MovementTypes.Walk)
                    {
                        var movementType = new GUIContent(typeProperty.enumDisplayNames[typeProperty.enumValueIndex].ToLower());
                        EditorGUILayout.LabelField(movementType, GUILayout.Width(GUI.skin.label.CalcSize(movementType).x - 3));
                    }
                    if (i != 0)
                    {
                        var icon = EditorGUIUtility.IconContent("P4_DeletedLocal");
                        if (GUILayout.Button(icon, GUI.skin.label, GUILayout.Width(GUI.skin.label.CalcSize(icon).x)))
                        {
                            toDelete = i;
                        }
                    }
                }
                if (toDelete != -1)
                {
                    movementSerializedProperty.DeleteArrayElementAtIndex(toDelete);
                }
                GUILayout.FlexibleSpace();
                var rect = EditorGUILayout.GetControlRect(GUILayout.Width(50));
                if (GUI.Button(rect, "Add", EditorStyles.miniPullDown))
                {
                    var menu = new GenericMenu();
                    var movementTypes = System.Enum.GetNames(typeof(MovementTypes));

                    for (int i = 0; i < movementTypes.Length; i++)
                    {
                        int k = i;
                        if (HasMovement(movementSerializedProperty, k))
                        {
                            menu.AddDisabledItem(new GUIContent(movementTypes[k]));
                        }
                        else
                        {
                            menu.AddItem(new GUIContent(movementTypes[k]), false, delegate
                            {
                                using var newMovementProperty = serializedObject.FindProperty("_movement");
                                var size = newMovementProperty.arraySize;
                                newMovementProperty.arraySize = size + 1;
                                using (var movementElementProperty = newMovementProperty.GetArrayElementAtIndex(size))
                                {
                                    using var distanceProperty = movementElementProperty.FindPropertyRelative("Distance");
                                    distanceProperty.intValue = 30;
                                    using var typeProperty = movementElementProperty.FindPropertyRelative("Type");
                                    typeProperty.enumValueIndex = k;
                                }
                            });
                        }
                    }
                    menu.DropDown(rect);
                }
            }
            bool HasMovement(SerializedProperty movementArrayProperty, int type)
            {
                for (int i = 0; i < movementArrayProperty.arraySize; i++)
                {
                    using var elementProperty = movementArrayProperty.GetArrayElementAtIndex(i);
                    using var typeProperty = elementProperty.FindPropertyRelative("Type");
                    if (typeProperty.enumValueIndex == type)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private ActionListDrawer _actionDataEditor;
        private readonly GUIContent _feetLabel = new GUIContent("ft.");
        private readonly Color _yellow = new Color(0.824f, 0.604f, 0.220f, 1f);
    }
}