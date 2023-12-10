using System.Collections.Generic;
using UnityEditor;

namespace SummonsTracker.EditorUtilities
{
    public static class EditorUtils
    {
#if UNITY_EDITOR
        public static IEnumerable<SerializedProperty> GetPropertiesInArray(this SerializedProperty serializedProperty)
        {
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                yield return serializedProperty.GetArrayElementAtIndex(i);
            }
        }
#endif
    }
}