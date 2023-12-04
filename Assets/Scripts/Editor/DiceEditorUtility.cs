using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Rolling
{
    public class DiceEditorUtility
    {
        public static void DrawDice(Rect position, string label, SerializedProperty diceProperty) =>
            DrawDice(position, new GUIContent(label), diceProperty);
        public static void DrawDice(Rect position, GUIContent label, SerializedProperty damageProp)
        {
            var dfacesProp = damageProp.FindPropertyRelative("_faces");
            var dnumberProp = damageProp.FindPropertyRelative("_number");
            var dmodProp = damageProp.FindPropertyRelative("_modifiers");
            var dfaces = dfacesProp.intValue;
            var dnumber = dnumberProp.intValue;
            var dmod = dmodProp.intValue;

            DrawDice(position, label,  ref dnumber, ref dfaces,ref dmod);
            
            dfacesProp.intValue = dfaces;
            dnumberProp.intValue = dnumber;
            dmodProp.intValue = dmod;
        }
        public static void DrawDice(Rect position, string label, ref int number, ref int faces, ref int modifier) =>
            DrawDice(position, new GUIContent(label), ref number, ref faces, ref modifier);
        public static void DrawDice(Rect position, GUIContent label, ref int number, ref int faces, ref int modifier)
        {
            var oldPrefixWidth = EditorGUIUtility.labelWidth;

            var nw = string.IsNullOrEmpty(label.text) && label.image == null ? 0f : EditorGUIUtility.labelWidth;
            var dw = EditorStyles.label.CalcSize(diceLabel).x-1;
            var pw = EditorStyles.label.CalcSize(plusLabel).x-1;
            var lw = (position.width - dw - pw - nw) / 3f;

            number = EditorGUI.IntField(new Rect(position.x, position.y, nw + lw, position.height), label, number);
            EditorGUIUtility.labelWidth = dw;
            faces = EditorGUI.IntField(new Rect(position.x + nw + lw, position.y, lw + dw, position.height), diceLabel, faces);
            EditorGUIUtility.labelWidth = pw;
            modifier = EditorGUI.IntField(new Rect(position.x + nw + lw + dw+ lw, position.y, lw + pw, position.height), plusLabel, modifier);

            EditorGUIUtility.labelWidth = oldPrefixWidth;
        }
        public static void DrawDiceLayout(GUIContent label, ref int number, ref int faces, ref int modifier) =>
            DrawDice(EditorGUILayout.GetControlRect(false), label, ref number, ref faces, ref modifier);
        public static void DrawDiceLayout(string label, ref int number, ref int faces, ref int modifier) => 
            DrawDice(EditorGUILayout.GetControlRect(false), new GUIContent(label), ref number, ref faces, ref modifier);

        private static readonly GUIContent diceLabel = new GUIContent("d");
        private static readonly GUIContent plusLabel = new GUIContent("+");
    }
}