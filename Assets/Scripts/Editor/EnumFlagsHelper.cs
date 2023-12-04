using System;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker
{
    public static class EnumFlagsHelper
    {
        public static void DrawEnumFlags(SerializedObject serializedObject,
                                         string serializedPropertyName,
                                         string prefixLabel = "",
                                         string noneLabel = "",
                                         Action<GenericMenu, SerializedProperty, int> onAddMenu = null,
                                         Func<int, string, string> getName = null)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                DrawEnumFlags(position, serializedObject, serializedPropertyName, prefixLabel, noneLabel, onAddMenu, getName);
            }
        }

        public static void DrawEnumFlags(Rect position,
                                         SerializedObject serializedObject,
                                         string serializedPropertyName,
                                         string prefixLabel = "",
                                         string noneLabel = "",
                                         Action<GenericMenu, SerializedProperty, int> onAddMenu = null,
                                         Func<int, string, string> getName = null)
            => DrawEnumFlags(position, serializedObject.FindProperty(serializedPropertyName), prefixLabel, noneLabel, onAddMenu, getName);


        public static void DrawEnumFlags(Rect position,
                                         SerializedProperty serializedProperty,
                                         string prefixLabel = "",
                                         string noneLabel = "",
                                         Action<GenericMenu, SerializedProperty, int> onAddMenu = null,
                                         Func<int, string, string> getName = null)
        {
            DrawEnumFlags(position, serializedProperty, new GUIContent(string.IsNullOrEmpty(prefixLabel) ? serializedProperty.displayName : noneLabel), noneLabel, onAddMenu, getName);
        }
        public static void DrawEnumFlags(SerializedObject serializedObject,
                                         string serializedPropertyName,
                                         GUIContent prefixLabel,
                                         string noneLabel = "",
                                         Action<GenericMenu, SerializedProperty, int> onAddMenu = null,
                                         Func<int, string, string> getName = null)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                DrawEnumFlags(position, serializedObject, serializedPropertyName, prefixLabel, noneLabel, onAddMenu, getName);
            }
        }

        public static void DrawEnumFlags(Rect position,
                                         SerializedObject serializedObject,
                                         string serializedPropertyName,
                                         GUIContent prefixLabel,
                                         string noneLabel = "",
                                         Action<GenericMenu, SerializedProperty, int> onAddMenu = null,
                                         Func<int, string, string> getName = null)
            => DrawEnumFlags(position, serializedObject.FindProperty(serializedPropertyName), prefixLabel, noneLabel, onAddMenu, getName);


        public static void DrawEnumFlags(Rect position,
                                         SerializedProperty serializedProperty,
                                         GUIContent prefixLabel,
                                         string noneLabel = "",
                                         Action<GenericMenu, SerializedProperty, int> onAddMenu = null,
                                         Func<int, string, string> getName = null)
        {
            if (!string.IsNullOrEmpty(prefixLabel.text) || prefixLabel.image != null)
            {
                position = EditorGUI.PrefixLabel(position, prefixLabel);
            }
            var currentFlags = serializedProperty.intValue;
            var label = string.Empty;
            for (int i = 1; i < serializedProperty.enumNames.Length; i++)
            {
                var k = Mathf.FloorToInt(Mathf.Pow(2, i - 1));
                if ((k & currentFlags) != 0)
                {
                    var current = serializedProperty.enumDisplayNames[i];
                    if (getName != null)
                    {
                        current = getName(k, current);
                    }
                    label = string.IsNullOrEmpty(label) ? current : $"{label}, {current}";
                }
            }
            if (GUI.Button(position, string.IsNullOrEmpty(label) ? string.IsNullOrEmpty(noneLabel) ? "—" : noneLabel : label, EditorStyles.miniPullDown))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"),
                    currentFlags == 0,
                    delegate
                    {
                        serializedProperty.intValue = 0;
                        serializedProperty.serializedObject.ApplyModifiedProperties();
                    });
                menu.AddSeparator(string.Empty);
                for (int i = 1; i < serializedProperty.enumNames.Length; i++)
                {
                    var k = Mathf.FloorToInt(Mathf.Pow(2, i - 1));

                    onAddMenu?.Invoke(menu, serializedProperty, k);
                    var current = serializedProperty.enumDisplayNames[i];
                    if (getName != null)
                    {
                        current = getName(k, current);
                    }

                    menu.AddItem(new GUIContent(current),
                        (k & currentFlags) != 0,
                        delegate
                        {
                            if ((k & currentFlags) != 0)
                            {
                                serializedProperty.intValue &= ~k;
                            }
                            else
                            {
                                serializedProperty.intValue |= k;
                            }
                            serializedProperty.serializedObject.ApplyModifiedProperties();
                        });
                }
                menu.DropDown(position);
            }
        }
    }
}