using SummonsTracker.Characters;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace SummonsTracker.Importer
{
    public static class ActionDataParser
    {
        public static bool TryMakeAction(SerializedProperty actionsSerializedProperty, int index, string title, string body, out ActionData actionData)
        {
            var inst = ScriptableObject.CreateInstance<ActionData>();
            using (var serializedObject = new SerializedObject(inst))
            {
                using (var noteProperty = serializedObject.FindProperty("_note"))
                {
                    noteProperty.stringValue = GetNote(body);
                }
                serializedObject.ApplyModifiedProperties();
            }
            actionData = inst;
            return true;
        }
        public static string GetNote(string rawText)
        {
            return Regex.Replace(rawText.Trim(), "<.*?>", string.Empty);
        }
    }
}