using System.Linq;
using SummonsTracker.EditorUtilities;
using SummonsTracker.Loading;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.LoadingEditor
{
    [CustomEditor(typeof(UniversalLoader))]
    public class UniversalLoaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SerializedProperty objectsProperty = serializedObject.FindProperty("objects");
            if (objectsProperty.arraySize > 0)
            {
                IOrderedEnumerable<IGrouping<string, SerializedProperty>> grouped = objectsProperty.GetPropertiesInArray()
                                                                                                   .Where(o => o != null)
                                                                                                   .Where(o => o.propertyType == SerializedPropertyType.ObjectReference)
                                                                                                   .Where(o => o.objectReferenceValue != null)
                                                                                                   .GroupBy(p => p.objectReferenceValue.GetType().Name)
                                                                                                   .OrderBy(delegate (IGrouping<string, SerializedProperty> g)
                                                                                                   {
                                                                                                       SerializedProperty p = g.FirstOrDefault();
                                                                                                       Object o = p.objectReferenceValue;
                                                                                                       if (!(o is ILoadable l))
                                                                                                       {
                                                                                                           return 0;
                                                                                                       }
                                                                                                       return l.Priority;
                                                                                                   }
                                                                                                   );
                foreach (IGrouping<string, SerializedProperty> group in grouped)
                {
                    EditorGUILayout.LabelField(group.Key, EditorStyles.boldLabel);
                    int c = 0;

                    SerializedProperty first = group.FirstOrDefault();
                    if (first != null)
                    {
                        var l = first.objectReferenceValue as ILoadable;
                        if (l != null)
                        {
                            EditorGUILayout.LabelField("Priority", l.Priority.ToString());
                        }
                    }
                    EditorGUI.indentLevel += 1;
                    foreach (SerializedProperty prop in group)
                    {
                        //EditorGUILayout.PropertyField(prop, new GUIContent("Element " + (++c).ToString()));
                        EditorGUILayout.ObjectField(new GUIContent("Element " + (++c).ToString()), prop.objectReferenceValue, typeof(Object), false);
                    }
                    EditorGUI.indentLevel -= 1;

                    EditorGUILayout.Space();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No Loadables have been found.\n\nPress \"Find Loadables\" to automatically find all Loadables in the project.", MessageType.Warning);
            }
            if (GUILayout.Button("Find Loadables"))
            {
                objectsProperty.arraySize = 0;
                Object[] objects = UniversalLoader.GetLoadablesInProject(false);
                if (objects != null && objects.Length >= 0)
                {
                    objectsProperty.arraySize = objects.Length;
                    for (int i = 0; i < objects.Length; i++)
                    {
                        objectsProperty.GetArrayElementAtIndex(i).objectReferenceValue = objects[i];
                    }
                    objectsProperty.serializedObject.ApplyModifiedProperties();
                }
            }
        }
        [MenuItem("CONTEXT/UniversalLoader/Refresh")]
        private static void ContextRefresh(MenuCommand command)
        {
            if (command.context == null)
            {
                return;
            }
            var serializedObject = new SerializedObject(command.context);
            SerializedProperty objectsProperty = serializedObject.FindProperty("objects");
            objectsProperty.arraySize = 0;

            Object[] objects = UniversalLoader.GetLoadablesInProject(false);
            if (objects != null && objects.Length >= 0)
            {
                objectsProperty.arraySize = objects.Length;
                for (int i = 0; i < objects.Length; i++)
                {
                    objectsProperty.GetArrayElementAtIndex(i).objectReferenceValue = objects[i];
                }
                objectsProperty.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
