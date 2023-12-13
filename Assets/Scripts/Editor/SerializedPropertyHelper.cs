using UnityEditor;

namespace SummonsTracker.EditorUtilities
{
    public static class SerializedPropertyHelper
    {
        public static void AssignEnumObj(this SerializedProperty property, object value) =>
            AssignEnumByName(property, value.ToString());
        public static void AssignEnumByName(this SerializedProperty property, string enumName)
        {
            var names = property.enumNames;
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == enumName.ToString())
                {
                    property.enumValueIndex = i;
                    break;
                }
            }
        }
    }
}