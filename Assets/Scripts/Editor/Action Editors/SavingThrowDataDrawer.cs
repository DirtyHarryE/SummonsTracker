using SummonsTracker.Rolling;
using System;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public class SavingThrowDataDrawer : ActionDrawer<SavingThrowData>
    {
        public SavingThrowDataDrawer(SavingThrowData target) : base(target) { }

        protected override void OnBeforeDrawNote()
        {
            DrawSavingThrow(GetRect, SerializedObject);
            base.OnBeforeDrawNote();
        }

        public static void DrawSavingThrow(Func<Rect> getRect, SerializedObject serializedObject)
        {
            var topLine = getRect();
            var dcSerializedProperty = serializedObject.FindProperty("_dc");
            var savingThrowSerializedProperty = serializedObject.FindProperty("_savingThrow");
            if (dcSerializedProperty == null || savingThrowSerializedProperty == null)
            {
                EditorGUI.DrawRect(topLine, Color.red);
                return;
            }
            var dcPreWidth = GUI.skin.label.CalcSize(_dcLabel).x;
            EditorGUIUtility.labelWidth = dcPreWidth;
            var dcRect = new Rect(topLine.x, topLine.y, dcPreWidth + 24, topLine.height);
            dcSerializedProperty.intValue = EditorGUI.IntField(dcRect, _dcLabel, dcSerializedProperty.intValue);

            var dcSavingThrowLabelWidth = GUI.skin.label.CalcSize(_savingThrowLabel).x;
            var savingThrowLabelRect = new Rect(topLine.x + topLine.width - dcSavingThrowLabelWidth, topLine.y, dcSavingThrowLabelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(savingThrowLabelRect, _savingThrowLabel);

            var ddX = topLine.x + dcRect.width + 2;
            var dropdownRect = new Rect(ddX, topLine.y, topLine.width - dcSavingThrowLabelWidth - dcRect.width - 4, EditorGUIUtility.singleLineHeight);
            var savingThrowValue = 0 <= savingThrowSerializedProperty.enumValueIndex && savingThrowSerializedProperty.enumValueIndex <= savingThrowSerializedProperty.enumNames.Length - 1 ?
                savingThrowSerializedProperty.enumDisplayNames[savingThrowSerializedProperty.enumValueIndex]
                : "error";
            if (GUI.Button(dropdownRect, savingThrowValue, EditorStyles.miniPullDown))
            {
                var menu = new GenericMenu();
                for (int i = 1; i < savingThrowSerializedProperty.enumDisplayNames.Length; i++)
                {
                    int k = Mathf.FloorToInt(Mathf.Pow(2, i - 1));
                    menu.AddItem(new GUIContent(savingThrowSerializedProperty.enumDisplayNames[i]), k == savingThrowSerializedProperty.intValue, delegate
                    {
                        savingThrowSerializedProperty.intValue = k;
                        savingThrowSerializedProperty.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.DropDown(topLine);
            }
            var onFailureLabelWidth = GUI.skin.label.CalcSize(_onFailureLabel).x;
            var onSuccessLabelWidth = GUI.skin.label.CalcSize(_onSuccessLabel).x;
            var onFailSuccLabelWidth = Mathf.Max(onSuccessLabelWidth, onFailureLabelWidth);
            var failRect = getRect();
            EditorGUI.LabelField(new Rect(failRect.x, failRect.y, onFailSuccLabelWidth, failRect.height), _onFailureLabel);
            var onFailOutcomeProperty = serializedObject.FindProperty("_failOutcome");
            DrawFailOutcome(failRect, onFailSuccLabelWidth, getRect, onFailOutcomeProperty);
            var succRect = getRect();
            EditorGUI.LabelField(new Rect(succRect.x, succRect.y, onFailSuccLabelWidth, succRect.height), _onSuccessLabel);
            DrawSuccessOutcome(succRect, onFailSuccLabelWidth, getRect, onFailOutcomeProperty, serializedObject.FindProperty("_successOutcome"));
        }

        private static void DrawFailOutcome(Rect position, float width, Func<Rect> getRect, SerializedProperty onFailOutcomeProperty)
        {
            using var failEnumProperty = onFailOutcomeProperty.FindPropertyRelative("_failSaveType");
            var currentFailOutcome = (FailSavingThrowOutcomes)failEnumProperty.enumValueIndex;
            var smallWidth = Mathf.Min(100f, (position.width - width) * 0.4f);
            var failOutcomeRect = new Rect(x: position.x + width,
                                           y: position.y,
                                           width: currentFailOutcome != FailSavingThrowOutcomes.Condition && currentFailOutcome != FailSavingThrowOutcomes.DamageAndCondition
                                               ? position.width - width
                                               : smallWidth,
                                           height: position.height);
            if (GUI.Button(failOutcomeRect, failEnumProperty.enumDisplayNames[failEnumProperty.enumValueIndex], EditorStyles.miniPullDown))
            {
                var menu = new GenericMenu();
                for (int i = 0; i < failEnumProperty.enumDisplayNames.Length; i++)
                {
                    int k = i;
                    menu.AddItem(new GUIContent(failEnumProperty.enumDisplayNames[i]), k == failEnumProperty.enumValueIndex, delegate
                    {
                        using var onNewFailEnum = onFailOutcomeProperty.FindPropertyRelative("_failSaveType");
                        onNewFailEnum.enumValueIndex = k;
                        onNewFailEnum.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.DropDown(failOutcomeRect);
            }
            if (currentFailOutcome == FailSavingThrowOutcomes.Condition || currentFailOutcome == FailSavingThrowOutcomes.DamageAndCondition)
            {
                const int m = 2;
                var failConditionRect = new Rect(x: position.x + width + failOutcomeRect.width + m,
                                                 y: position.y,
                                                 width: position.width - width - smallWidth - m,
                                                 height: position.height);
                EnumFlagsHelper.DrawEnumFlags(failConditionRect, onFailOutcomeProperty.FindPropertyRelative("_condition"), GUIContent.none, noneLabel: "none");
            }
            if (currentFailOutcome == FailSavingThrowOutcomes.Damage || currentFailOutcome == FailSavingThrowOutcomes.DamageAndCondition)
            {
                var dmgRect = getRect();
                dmgRect = new Rect(dmgRect.x + width, dmgRect.y, dmgRect.width - width, dmgRect.height);

                using var failDiceProperty = onFailOutcomeProperty.FindPropertyRelative("_damage");
                using var failDamageTypeProp = onFailOutcomeProperty.FindPropertyRelative("_damageType");
                DiceEditorUtility.DrawDice(new Rect(dmgRect.x, dmgRect.y, dmgRect.width * 0.6f, dmgRect.height), GUIContent.none, failDiceProperty);

                if (GUI.Button(new Rect(4 + dmgRect.x + (dmgRect.width * 0.6f), dmgRect.y, (dmgRect.width * 0.4f) - 4, EditorGUIUtility.singleLineHeight), failDamageTypeProp.enumDisplayNames[failDamageTypeProp.enumValueIndex].ToLower(), EditorStyles.miniPullDown))
                {
                    var menu = new GenericMenu();
                    for (int i = 1; i < failDamageTypeProp.enumNames.Length; i++)
                    {
                        var k = i;
                        menu.AddItem(new GUIContent(failDamageTypeProp.enumDisplayNames[k]),
                            k == failDamageTypeProp.enumValueIndex,
                            delegate
                            {
                                using var newFailDamageTypeProp = onFailOutcomeProperty.FindPropertyRelative("_damageType");
                                newFailDamageTypeProp.enumValueIndex = k;
                                newFailDamageTypeProp.serializedObject.ApplyModifiedProperties();
                            });
                    }
                    menu.DropDown(dmgRect);
                }
            }
            if (currentFailOutcome != FailSavingThrowOutcomes.Nothing)
            {
                using var noteProperty = onFailOutcomeProperty.FindPropertyRelative("_outcomeNote");
                var noteRect = getRect();
                noteRect = new Rect(noteRect.x + width, noteRect.y, noteRect.width - width, noteRect.height);
                noteProperty.stringValue = EditorGUI.TextField(noteRect, noteProperty.stringValue);
            }
        }

        private static void DrawSuccessOutcome(Rect position, float width, Func<Rect> getRect, SerializedProperty onFailOutcomeProperty, SerializedProperty onSuccessOutcomeProperty)
        {
            using var SuccessEnumProperty = onSuccessOutcomeProperty.FindPropertyRelative("_successSaveType");
            var currentSuccessOutcome = (SuccessSavingThrowOutcomes)SuccessEnumProperty.enumValueIndex;
            var smallWidth = Mathf.Min(100f, (position.width - width) * 0.4f);
            var SuccessOutcomeRect = new Rect(x: position.x + width,
                                           y: position.y,
                                           width: currentSuccessOutcome != SuccessSavingThrowOutcomes.Condition && currentSuccessOutcome != SuccessSavingThrowOutcomes.DamageAndCondition
                                               ? position.width - width
                                               : smallWidth,
                                           height: position.height);
            if (GUI.Button(SuccessOutcomeRect, SuccessEnumProperty.enumDisplayNames[SuccessEnumProperty.enumValueIndex], EditorStyles.miniPullDown))
            {
                using var failEnumProperty = onFailOutcomeProperty.FindPropertyRelative("_failSaveType");
                var menu = new GenericMenu();
                for (int i = 0; i < SuccessEnumProperty.enumDisplayNames.Length; i++)
                {
                    int k = i;
                    var failType = (FailSavingThrowOutcomes)failEnumProperty.enumValueIndex;
                    var successType = (SuccessSavingThrowOutcomes)k;
                    if (successType == SuccessSavingThrowOutcomes.Half && !(failType == FailSavingThrowOutcomes.Damage || failType == FailSavingThrowOutcomes.DamageAndCondition))
                    {
                        menu.AddDisabledItem(new GUIContent(SuccessEnumProperty.enumDisplayNames[i]), k == SuccessEnumProperty.enumValueIndex);
                    }
                    else
                    {
                        menu.AddItem(new GUIContent(SuccessEnumProperty.enumDisplayNames[i]), k == SuccessEnumProperty.enumValueIndex, delegate
                        {
                            using var onNewSuccessEnum = onSuccessOutcomeProperty.FindPropertyRelative("_successSaveType");
                            onNewSuccessEnum.enumValueIndex = k;
                            onNewSuccessEnum.serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
                menu.DropDown(SuccessOutcomeRect);
            }
            if (currentSuccessOutcome == SuccessSavingThrowOutcomes.Condition || currentSuccessOutcome == SuccessSavingThrowOutcomes.DamageAndCondition)
            {
                const int m = 2;
                var SuccessConditionRect = new Rect(x: position.x + width + SuccessOutcomeRect.width + m,
                                                 y: position.y,
                                                 width: position.width - width - smallWidth - m,
                                                 height: position.height);
                EnumFlagsHelper.DrawEnumFlags(SuccessConditionRect, onSuccessOutcomeProperty.FindPropertyRelative("_condition"), GUIContent.none, noneLabel: "none");
            }
            if (currentSuccessOutcome == SuccessSavingThrowOutcomes.Damage || currentSuccessOutcome == SuccessSavingThrowOutcomes.DamageAndCondition)
            {
                var dmgRect = getRect();
                dmgRect = new Rect(dmgRect.x + width, dmgRect.y, dmgRect.width - width, dmgRect.height);

                using var SuccessDiceProperty = onSuccessOutcomeProperty.FindPropertyRelative("_damage");
                using var SuccessDamageTypeProp = onSuccessOutcomeProperty.FindPropertyRelative("_damageType");
                DiceEditorUtility.DrawDice(new Rect(dmgRect.x, dmgRect.y, dmgRect.width * 0.6f, dmgRect.height), GUIContent.none, SuccessDiceProperty);

                if (GUI.Button(new Rect(4 + dmgRect.x + (dmgRect.width * 0.6f), dmgRect.y, (dmgRect.width * 0.4f) - 4, EditorGUIUtility.singleLineHeight), SuccessDamageTypeProp.enumDisplayNames[SuccessDamageTypeProp.enumValueIndex].ToLower(), EditorStyles.miniPullDown))
                {
                    var menu = new GenericMenu();
                    for (int i = 1; i < SuccessDamageTypeProp.enumNames.Length; i++)
                    {
                        var k = i;
                        menu.AddItem(new GUIContent(SuccessDamageTypeProp.enumDisplayNames[k]),
                            k == SuccessDamageTypeProp.enumValueIndex,
                            delegate
                            {
                                using var newSuccessDamageTypeProp = onSuccessOutcomeProperty.FindPropertyRelative("_damageType");
                                newSuccessDamageTypeProp.enumValueIndex = k;
                                newSuccessDamageTypeProp.serializedObject.ApplyModifiedProperties();
                            });
                    }
                    menu.DropDown(dmgRect);
                }
            }
            if (currentSuccessOutcome != SuccessSavingThrowOutcomes.Nothing)
            {
                using var noteProperty = onSuccessOutcomeProperty.FindPropertyRelative("_outcomeNote");
                var noteRect = getRect();
                noteRect = new Rect(noteRect.x + width, noteRect.y, noteRect.width - width, noteRect.height);
                noteProperty.stringValue = EditorGUI.TextField(noteRect, noteProperty.stringValue);
            }
        }

        private static readonly GUIContent _dcLabel = new GUIContent("DC");
        private static readonly GUIContent _savingThrowLabel = new GUIContent("Saving Throw");
        private static readonly GUIContent _onFailureLabel = new GUIContent("On Failure");
        private static readonly GUIContent _onSuccessLabel = new GUIContent("On Success");
    }
}