using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public class MultiattackDataDrawer : ActionDrawer<MultiattackData>
    {
        public MultiattackDataDrawer(MultiattackData target, SerializedObject baseSerializedObject) : base(target)
        {
            _baseSerializedObject = baseSerializedObject;
        }

        protected override float NoteSize => EditorGUIUtility.singleLineHeight;

        protected override void OnBeforeDrawNote()
        {
            DrawMultiAttack();
            base.OnBeforeDrawNote();
        }

        private void DrawMultiAttack()
        {
            var deleteSkin = GUI.skin.label;
            var delete = EditorGUIUtility.IconContent("TreeEditor.Trash");
            var deleteButtonSize = deleteSkin.CalcSize(delete);
            var defaultHeight = Mathf.Max(EditorGUIUtility.singleLineHeight, deleteButtonSize.y);
            using var actionsProperty = _baseSerializedObject.FindProperty("_actions");
            using var attacksProperty = SerializedObject.FindProperty("_attacks");

            for (int i = 0; i < attacksProperty.arraySize; i++)
            {
                int j = i;
                using var attackProperty = attacksProperty.GetArrayElementAtIndex(j);

                var position = GetRect(defaultHeight);

                var lw = position.width * 0.6f;
                var lRect = new Rect(position.x, position.y, lw, position.height);
                var rRect = new Rect(position.x + lw, position.y, position.width - lw - deleteButtonSize.x - 2, position.height);
                var lButtonRect = new Rect(position.x, position.y, lw - 2, position.height);
                using var attackIndexProperty = attackProperty.FindPropertyRelative("_attackIndex");
                var buttonLabel = "Any";
                if (0 <= attackIndexProperty.intValue && attackIndexProperty.intValue < actionsProperty.arraySize)
                {
                    using var referencedActionProperty = actionsProperty.GetArrayElementAtIndex(attackIndexProperty.intValue);
                    buttonLabel = referencedActionProperty.objectReferenceValue.name;
                }

                if (GUI.Button(lButtonRect, buttonLabel, EditorStyles.miniPullDown))
                {
                    DrawActionDropdownMenu(lButtonRect, attackIndexProperty.intValue, actionsProperty, k =>
                    {
                        using var newAttacksProperty = SerializedObject.FindProperty("_attacks");
                        using var newAttackProperty = newAttacksProperty.GetArrayElementAtIndex(j);
                        using var newAttackIndexProperty = newAttackProperty.FindPropertyRelative("_attackIndex");
                        newAttackIndexProperty.intValue = k;
                        newAttackIndexProperty.serializedObject.ApplyModifiedProperties();
                    }, o => AllowMenuItem(o, j));
                }
                using var attackAmountProperty = attackProperty.FindPropertyRelative("_attackNumber");
                var newAmt = EditorGUI.IntField(rRect, attackAmountProperty.intValue);
                if (newAmt != attackAmountProperty.intValue)
                {
                    attackAmountProperty.intValue = newAmt;
                    attackAmountProperty.serializedObject.ApplyModifiedProperties();
                }
                if (GUI.Button(new Rect(position.x + lw + rRect.width, position.y, deleteButtonSize.x, position.height), delete, deleteSkin))
                {
                    attacksProperty.DeleteArrayElementAtIndex(j);
                }
            }

            var addRect = GetRect();
            if (GUI.Button(addRect, "Add", EditorStyles.miniPullDown))
            {
                DrawActionDropdownMenu(addRect, -10, actionsProperty, k =>
                {
                    using (var newAttacksProperty = SerializedObject.FindProperty("_attacks"))
                    {
                        var size = newAttacksProperty.arraySize;
                        newAttacksProperty.arraySize = size + 1;
                        using (var newAttackProperty = newAttacksProperty.GetArrayElementAtIndex(size))
                        {
                            using (var newActionIndexProperty = newAttackProperty.FindPropertyRelative("_attackIndex"))
                            {
                                newActionIndexProperty.intValue = k;
                            }
                        }
                        newAttacksProperty.serializedObject.ApplyModifiedProperties();
                    }
                }, o => AllowMenuItem(o));
            }
        }

        private bool AllowMenuItem(UnityEngine.Object obj, int currentIndex = -1)
        {
            using var actionsProperty = _baseSerializedObject.FindProperty("_actions");
            using var attacksProperty = SerializedObject.FindProperty("_attacks");
            if (0 <= currentIndex  && currentIndex < attacksProperty.arraySize)
            {
                using var currentAttackProperty = attacksProperty.GetArrayElementAtIndex(currentIndex);
                using var currentAttackIndexProperty = currentAttackProperty.FindPropertyRelative("_attackIndex");
                if (0 <= currentAttackIndexProperty.intValue && currentAttackIndexProperty.intValue < actionsProperty.arraySize)
                {
                    using var currentActionProperty = actionsProperty.GetArrayElementAtIndex(currentAttackIndexProperty.intValue);
                    if (obj == currentActionProperty.objectReferenceValue)
                    {
                        return true;
                    }
                }
            }


            for (int l = 0; l < attacksProperty.arraySize; l++)
            {
                using var attackProperty = attacksProperty.GetArrayElementAtIndex(l);
                using var attackIndexProperty = attackProperty.FindPropertyRelative("_attackIndex");
                if (0 <= attackIndexProperty.intValue && attackIndexProperty.intValue < actionsProperty.arraySize)
                {
                    using var actionProperty = actionsProperty.GetArrayElementAtIndex(attackIndexProperty.intValue);
                    if (obj == actionProperty.objectReferenceValue)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void DrawActionDropdownMenu(Rect rect, int currentIndex, SerializedProperty actionsProperty, Action<int> onSelect, Func<UnityEngine.Object, bool> shouldAdd)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Any"), currentIndex == -1, () => onSelect(-1));
            for (int i = 0; i < actionsProperty.arraySize; i++)
            {
                var k = i;
                using (var newReferencedActionProperty = actionsProperty.GetArrayElementAtIndex(k))
                {
                    var obj = newReferencedActionProperty.objectReferenceValue;

                    if ((obj is AttackData || obj is ISavingThrow) && shouldAdd(obj))
                    {
                        menu.AddItem(new GUIContent(obj.name),
                        k == currentIndex,
                        delegate
                        {
                            onSelect(k);
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(obj.name), k == currentIndex);
                    }
                }
            }
            menu.DropDown(rect);
        }

        private SerializedObject _baseSerializedObject;
    }
}