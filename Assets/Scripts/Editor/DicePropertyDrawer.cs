using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Rolling
{
    [CustomPropertyDrawer(typeof(Dice))]
    public class DicePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using var numberProperty = property.FindPropertyRelative("_number");
            using var facesProperty = property.FindPropertyRelative("_faces");
            using var modifiersProperty = property.FindPropertyRelative("_modifiers");

            var number = numberProperty.intValue;
            var faces = facesProperty.intValue;
            var mod = modifiersProperty.intValue;

            DiceEditorUtility.DrawDice(position, label, ref number, ref faces, ref mod);

            numberProperty.intValue = number ;
            facesProperty.intValue = faces;
            modifiersProperty.intValue = mod;
        }
    }
}