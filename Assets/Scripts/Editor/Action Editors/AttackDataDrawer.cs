using SummonsTracker.Rolling;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public class AttackDataDrawer : ActionDrawer<AttackData>
    {
        public AttackDataDrawer(AttackData target) : base(target) { }

        protected override void OnBeforeDrawNote()
        {
            DrawAttackType();
            DrawDamageDice(SerializedObject.FindProperty("_damage"), SerializedObject.FindProperty("_damageType"));
        }

        protected void DrawAttackType()
        {
            var topLine = GetRect();

            var attackTypeProp = SerializedObject.FindProperty("_attackType");
            var atkTypeName = attackTypeProp.enumDisplayNames[attackTypeProp.enumValueIndex];
            if (GUI.Button(topLine, atkTypeName, EditorStyles.miniPullDown))
            {
                var menu = new GenericMenu();
                for (int i = 0; i < attackTypeProp.enumNames.Length; i++)
                {
                    var k = i;
                    menu.AddItem(new GUIContent(attackTypeProp.enumDisplayNames[k]),
                        k == attackTypeProp.enumValueIndex,
                        delegate
                        {
                            attackTypeProp.enumValueIndex = k;
                            attackTypeProp.serializedObject.ApplyModifiedProperties();
                        });
                }
                menu.DropDown(topLine);
            }

            var position = GetRect();
            const int fieldW = 16;

            var modProp = SerializedObject.FindProperty("_attackMod");
            var modPreW = GUI.skin.label.CalcSize(_plusLabel).x - 4;
            EditorGUIUtility.labelWidth = modPreW;
            var modW = modPreW + fieldW;
            modProp.intValue = EditorGUI.IntField(new Rect(position.x, position.y, modW, EditorGUIUtility.singleLineHeight), _plusLabel, modProp.intValue);
            var hitW = GUI.skin.label.CalcSize(_toHitLabel).x;
            EditorGUI.LabelField(new Rect(position.x + modW, position.y, hitW, EditorGUIUtility.singleLineHeight), _toHitLabel);

            var reachProp = SerializedObject.FindProperty("_range");
            var reachPreW = GUI.skin.label.CalcSize(_reachLabel).x;
            EditorGUIUtility.labelWidth = reachPreW;
            var reachW = reachPreW + (fieldW * 1.5f);
            if (attackTypeProp.enumNames[attackTypeProp.enumValueIndex] == AttackType.RangedSpellAttack.ToString())
            {
                reachW = reachPreW + (fieldW * 2f);
            }
            reachProp.intValue = EditorGUI.IntField(new Rect(position.x + modW + hitW, position.y, reachW, EditorGUIUtility.singleLineHeight), _reachLabel, reachProp.intValue);
            if (attackTypeProp.enumNames[attackTypeProp.enumValueIndex] == AttackType.RangedWeaponAttack.ToString())
            {
                var maxReachProp = SerializedObject.FindProperty("_maxRange");
                var maxReachPreW = GUI.skin.label.CalcSize(_maxReachLabel).x - 2;
                EditorGUIUtility.labelWidth = maxReachPreW;
                var maxReachW = maxReachPreW + (fieldW * 2f);
                maxReachProp.intValue = EditorGUI.IntField(new Rect(position.x + modW + hitW + reachW, position.y, maxReachW, EditorGUIUtility.singleLineHeight), _maxReachLabel, maxReachProp.intValue);
                reachW = reachW + maxReachW;
            }
            var feetW = GUI.skin.label.CalcSize(_feetLabel).x;
            EditorGUI.LabelField(new Rect(position.x + modW + hitW + reachW, position.y, feetW, EditorGUIUtility.singleLineHeight), _feetLabel);

            const int advW = 32;
            var targetX = position.x + modW + hitW + reachW + feetW;
            var targetW = position.width - modW - hitW - reachW - feetW - advW;
            var targetProp = SerializedObject.FindProperty("_target");

            targetProp.stringValue = EditorGUI.TextField(new Rect(targetX, position.y, targetW - 2, EditorGUIUtility.singleLineHeight), targetProp.stringValue);

            DrawAdvButton(new Rect(targetX + targetW, position.y, advW, EditorGUIUtility.singleLineHeight), SerializedObject.FindProperty("_advantageType"));

        }

        protected void DrawDamageDice(SerializedProperty damageProp, SerializedProperty damageTypeProp)
        {
            var position = GetRect();
            var dfacesProp = damageProp.FindPropertyRelative("_faces");
            var dnumberProp = damageProp.FindPropertyRelative("_number");
            var dmodProp = damageProp.FindPropertyRelative("_modifiers");
            var dfaces = dfacesProp.intValue;
            var dnumber = dnumberProp.intValue;
            var dmod = dmodProp.intValue;
            var hitPreW = GUI.skin.label.CalcSize(_hitLabel).x;
            EditorGUIUtility.labelWidth = hitPreW;
            DiceEditorUtility.DrawDice(new Rect(position.x, position.y, position.width * 0.5f, EditorGUIUtility.singleLineHeight), "Hit", ref dnumber,ref dfaces,  ref dmod);
            dfacesProp.intValue = dfaces;
            dnumberProp.intValue = dnumber;
            dmodProp.intValue = dmod;

            if (GUI.Button(new Rect(4 + position.x + (position.width * 0.5f), position.y, (position.width * 0.5f) - 4, EditorGUIUtility.singleLineHeight), damageTypeProp.enumDisplayNames[damageTypeProp.enumValueIndex].ToLower(), EditorStyles.miniPullDown))
            {
                var menu = new GenericMenu();
                for (int i = 1; i < damageTypeProp.enumNames.Length; i++)
                {
                    var k = i;
                    menu.AddItem(new GUIContent(damageTypeProp.enumDisplayNames[k]),
                        k == damageTypeProp.enumValueIndex,
                        delegate
                        {
                            damageTypeProp.enumValueIndex = k;
                            damageTypeProp.serializedObject.ApplyModifiedProperties();
                        });
                }
                menu.DropDown(position);
            }
        }

        protected void DrawAdvButton(Rect position, SerializedProperty serializedProperty)
        {
            int index = serializedProperty.enumValueIndex;
            var content =
                index == 1 ? EditorGUIUtility.IconContent("winbtn_mac_max_h") :
                index == 2 ? EditorGUIUtility.IconContent("winbtn_mac_close_h") :
                EditorGUIUtility.IconContent("winbtn_mac_min");

            var down = EditorGUIUtility.IconContent("d_icon dropdown");
            var w = GUI.skin.label.CalcSize(down).x;

            GUI.Label(new Rect(position.x, position.y, position.width, position.height), content);
            GUI.Label(new Rect(position.x + position.width - w - 1, position.y + 2, w, position.height), down);

            if (GUI.Button(position, GUIContent.none, GUI.skin.label))
            {
                var menu = new GenericMenu();
                for (int i = 0; i < serializedProperty.enumNames.Length; i++)
                {
                    int k = i;
                    menu.AddItem(new GUIContent(serializedProperty.enumDisplayNames[k]), k == serializedProperty.enumValueIndex, delegate
                    {
                        serializedProperty.enumValueIndex = k;
                        serializedProperty.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.DropDown(position);
            }
        }


        private static readonly GUIContent _hitLabel = new GUIContent("Hit");
        private static readonly GUIContent _plusLabel = new GUIContent("+");
        private static readonly GUIContent _toHitLabel = new GUIContent("to hit,");
        private static readonly GUIContent _reachLabel = new GUIContent("reach");
        private static readonly GUIContent _maxReachLabel = new GUIContent("/");
        private static readonly GUIContent _feetLabel = new GUIContent("ft.");
    }
}