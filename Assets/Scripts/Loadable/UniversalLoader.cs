#if UNITY_EDITOR
#define USE_EDITOR
#endif

using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

using Object = UnityEngine.Object;

#if USE_EDITOR
using UnityEditor;
using UnityEditor.Build;

using System.Linq;
using SummonsTracker.EditorUtilities;
using UnityEditor.Build.Reporting;
#endif

namespace SummonsTracker.Loading
{
    [CreateAssetMenu(menuName = "Italic Pig/Paleo/Loader", fileName = "Loader")]
#if USE_EDITOR
#if AUTO_INITIALISE
    [InitializeOnLoad]
#endif
    public class UniversalLoader : ScriptableObject, IPreprocessBuildWithReport
#else
    public class UniversalLoader : ScriptableObject
#endif
    {
        private const string INTERVAL_PREF = "LOADER_UNITY_INTERVAL";
        private const string TIME_PREF = "LOADER_UNITY_UPDATE_TIME_CHECK";

        public static bool PauseImport = false;
        public static bool IsLoaded { get; private set; } = false;

        #region EDITOR
#if USE_EDITOR
        int IOrderedCallback.callbackOrder => 0;
        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            InitialiseLoaders(false, false);
        }

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
            => new SettingsProvider("Preferences/Universal Loader", SettingsScope.User)
            {
                label = "Universal Loader",
                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.LabelField("Time between Getting Loaders", EditorStyles.boldLabel);
                    EditorPrefs.SetFloat(INTERVAL_PREF,
                        EditorGUILayout.FloatField("Minutes",
                        Mathf.Max(0, EditorPrefs.GetFloat(INTERVAL_PREF, 30))));

                    if (EditorPrefs.HasKey(TIME_PREF))
                    {
                        EditorGUILayout.LabelField("Last Loader Refresh", EditorPrefs.GetString(TIME_PREF));
                    }
                },

                keywords = new HashSet<string>(new[] { "Universal Loader" })
            };

#if AUTO_INITIALISE
        [MenuItem("CONTEXT/UniversalLoader/Test Auto-Initialise (Time check)")]
        public static void AutoInitialise()
        {
            if (PauseImport)
            {
                return;
            }
            if (EditorApplication.isPlaying)
            {
                return;
            }

            if (EditorPrefs.HasKey(TIME_PREF))
            {
                string timeString = EditorPrefs.GetString(TIME_PREF);
                try
                {
                    DateTime dateTime = DateTime.Parse(timeString);
                    TimeSpan span = DateTime.Now.Subtract(dateTime);

                    if (span.TotalMinutes < EditorPrefs.GetFloat(INTERVAL_PREF, 5))
                    {
                        return;
                    }
                }
                catch (FormatException)
                {
                    return;
                }
            }
            string nowStr = DateTime.Now.ToString();
            EditorPrefs.SetString(TIME_PREF, nowStr);
            InitialiseLoaders(true);
        }
#endif
        [MenuItem("CONTEXT/UniversalLoader/Test Initialise")]
        public static void InitialiseLoaders() => InitialiseLoaders(true);
        public static void InitialiseLoaders(bool cancellable, bool trimExisting = true)
        {
            Debug.Log("Refreshing Loaders");

            UniversalLoader[] loaders = AssetDatabase.FindAssets("t:UniversalLoader")
                                                     .Select(AssetDatabase.GUIDToAssetPath)
                                                     .OrderBy(s => s)
                                                     .Select(AssetDatabase.LoadAssetAtPath<UniversalLoader>)
                                                     .ToArray();

            Object[] loadables = trimExisting
                ? GetLoadablesInProject(cancellable, loaders)
                : GetLoadablesInProject(cancellable);
            if (loadables == null || loadables.Length <= 0)
            {
                return;
            }
            IEnumerable<SerializedObject> serializedObjects = loaders.Select(u => new SerializedObject(u));
            foreach (SerializedObject serializedObject in serializedObjects)
            {
                SerializedProperty arr = serializedObject.FindProperty("objects");
                if (NeedsUpdating(serializedObject, loadables))
                {
                    arr.arraySize = loadables.Length;
                    for (int i = 0; i < loadables.Length; i++)
                    {
                        SerializedProperty entry = arr.GetArrayElementAtIndex(i);
                        entry.objectReferenceValue = loadables[i];
                    }
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        private static bool NeedsUpdating(SerializedObject loader, Object[] loadables)
        {
            SerializedProperty arr = loader.FindProperty("objects");
            if (arr.arraySize != loadables.Length)
            {
                return true;
            }

            var hash = new HashSet<Object>(loadables);

            for (int i = 0; i < arr.arraySize; i++)
            {
                Object obj = arr.GetArrayElementAtIndex(i).objectReferenceValue;
                if (obj == null)
                {
                    return true;
                }
                if (hash.Contains(obj))
                {
                    hash.Remove(obj);
                }
            }
            return hash.Count > 0;
        }
        public static Object[] GetLoadablesInProject(bool cancellable, params UniversalLoader[] loaders)
        {
            try
            {
                Type loaderType = typeof(ILoadable);
                IEnumerable<Type> loadingTypes = AppDomain.CurrentDomain.GetAssemblies()
                                                                        .SelectMany(a => a.GetTypes())
                                                                        .Where(t => loaderType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericType);
                string search = string.Join(" ", loadingTypes.Select(t => "t:" + t.Name).ToArray());

                var list = new List<Object>();

                var pathnameHash = new HashSet<string>();
                var guidHash = new HashSet<string>();
                var instanceIDHash = new HashSet<int>();

                int i, j;
                for (i = 0; i < loaders.Length; i++)
                {
                    IEnumerable<Object> alreadyLoaded = new SerializedObject(loaders[i]).FindProperty("objects")
                        .GetPropertiesInArray()
                        .Select(o => o.objectReferenceValue)
                        .Where(o => o != null);
                    foreach (Object obj in alreadyLoaded)
                    {
                        int instanceID = obj.GetInstanceID();
                        if (!instanceIDHash.Contains(instanceID))
                        {
                            instanceIDHash.Add(instanceID);
                        }
                        string path = AssetDatabase.GetAssetPath(obj);
                        if (!pathnameHash.Contains(path))
                        {
                            pathnameHash.Add(path);
                        }
                        string guid = AssetDatabase.AssetPathToGUID(path);
                        if (!guidHash.Contains(guid))
                        {
                            guidHash.Add(guid);
                        }
                        list.Add(obj);
                    }
                }

                string[] guids = AssetDatabase.FindAssets(string.IsNullOrEmpty(search) ? "t:ScriptableObject" : search);
                float max = guids.Length;

                for (i = 0; i < guids.Length; i++)
                {
                    if (guidHash.Contains(guids[i]))
                    {
                        continue;
                    }
                    guidHash.Add(guids[i]);
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    if (pathnameHash.Contains(path))
                    {
                        continue;
                    }
                    pathnameHash.Add(path);

                    if (cancellable)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Getting Loaders", path, i / max))
                        {
                            return null;
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayProgressBar("Getting Loaders", path, i / max);
                    }
                    try
                    {
                        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
                        for (j = 0; j < objects.Length; j++)
                        {
                            if (objects[j] == null)
                            {
                                continue;
                            }
                            int instanceID = objects[j].GetInstanceID();
                            if (instanceIDHash.Contains(instanceID))
                            {
                                continue;
                            }
                            instanceIDHash.Add(instanceID);
                            if (objects[j] is ILoadable || objects[j].GetType().IsSubclassOf(typeof(ILoadable)))
                            {
                                list.Add(objects[j]);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error during Universal Loader");
                        Debug.LogError(e);
                    }
                }
                EditorUtility.DisplayProgressBar("Getting Loaders", "Finalising", 1);

                list.Sort(CompareLoadables);
                return list.Distinct().ToArray();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return null;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }


        private static int CompareLoadables(Object a, Object b)
        {
            var lA = a as ILoadable;
            var lB = b as ILoadable;
            if (lA == lB)
            {
                return 0;
            }
            if (lA.Priority != lB.Priority)
            {
                return lA.Priority.CompareTo(lB.Priority);
            }
            string xType = lA.GetType().Name;
            string yType = lB.GetType().Name;
            if (xType != yType)
            {
                return xType.CompareTo(yType);
            }
            return GetObjectString(lA as Object).CompareTo(GetObjectString(lB as Object));
        }
        private static string GetObjectString(Object o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            return AssetDatabase.GetAssetPath(o) + "_" + o.name;
        }

#if AUTO_INITIALISE
        static UniversalLoader()
        {
            EditorApplication.update += OnUpdate;
            void OnUpdate()
            {
                EditorApplication.update -= OnUpdate;
                AutoInitialise();
            }
        }
#endif
#endif
        #endregion

        [SerializeField]
        private Object[] objects = Array.Empty<Object>();

        public ILoadable[] Loadables { get; private set; } = Array.Empty<ILoadable>();

        public void DoLoad()
        {
            if (IsLoaded)
            {
                return;
            }
            IsLoaded = true;
            if (Loadables == null || Loadables.Length == 0)
            {
                Loadables = GetLoadables();
            }
            var builder = new StringBuilder();
            builder.AppendLine("Loading");
            for (int i = 0; i < Loadables.Length; i++)
            {
                ILoadable l = Loadables[i];
                l.Load();
                builder.AppendLine(l.ToString());
            }
            Debug.Log(builder.ToString());
        }

        public ILoadable[] GetLoadables()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Get Loadable");
            var list = new List<ILoadable>();
            for (int i = 0; i < objects.Length; i++)
            {
                try
                {
                    if (objects[i] == null)
                    {
                        builder.Append("FAIL : [").Append(i).Append("] - IS NULL").AppendLine();
                        continue;
                    }
                    var loadable = objects[i] as ILoadable;
                    if (loadable != null)
                    {
                        list.Add(loadable);
                        builder.Append("Success : [").Append(i).Append("] - ").Append(loadable.ToString()).AppendLine();
                    }
                    else
                    {
                        builder.Append("FAIL : [").Append(i).Append("] - ").Append(objects[i].ToString()).Append(" :: IS NOT LOADABLE").AppendLine();
                    }
                }
                catch (Exception e)
                {
                    builder.Append("FAIL : [").Append(i).Append("] - ").Append(objects[i].ToString()).Append(" :: ").Append(e).AppendLine();
                }
            }
            Debug.Log(builder.ToString());
            return list.ToArray();
        }

#if USE_EDITOR
        private void Reset()
        {
            Object[] objs = GetLoadablesInProject(false);
            if (objs != null && objs.Length >= 0)
            {
                objects = objs;
            }
        }
#endif
        private void OnValidate()
        {
            var list = new List<Object>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] == null || !(objects[i] is ILoadable))
                {
                    continue;
                }
                list.Add(objects[i]);
            }
            objects = list.ToArray();
        }

    }
}
