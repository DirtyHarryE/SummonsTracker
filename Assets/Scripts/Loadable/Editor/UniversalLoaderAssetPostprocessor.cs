#if UNITY_EDITOR
#define USE_EDITOR
#endif
//#define DEBUG_LOG

using System.Collections.Generic;
using System.IO;
using System.Linq;
using SummonsTracker.EditorUtilities;
using SummonsTracker.Loading;
using UnityEditor;

namespace SummonsTracker.LoadingEditor
{
#if USE_EDITOR
    public class UniversalLoaderAssetPostprocessor : AssetPostprocessor
    {
        private static readonly HashSet<string> ignoredFiles = new HashSet<string>(new[]
        {
            "AudioManager",
            "ClusterInputManager",
            "DynamicsManager",
            "EditorBuildSettings",
            "GraphicsSettings",
            "EditorSettings",
            "InputManager",
            "NavMeshAreas",
            "PackageManagerSettings",
            "Physics2DSettings",
            "PresetManager",
            "ProjectSettings",
            "QualitySettings",
            "TagManager",
            "TimeManager",
            "UnityConnectSettings",
            "URPProjectSettings",
            "VersionControlSettings",
            "VFXManager",
            "XRSettings",
            "UniversalRenderPipelineAsset_Renderer"
        });

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (Check(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths))
            {
                UniversalLoader.InitialiseLoaders();
            }
        }
        private static bool Check(params string[][] arrays)
        {
            HashSet<string> alreadyLoaded = null;
            if (arrays == null || arrays.Length <= 0)
            {
                return false;
            }
#if DEBUG_LOG
            var builder = new StringBuilder();
#endif
            try
            {
                const string assetPref = "Assets/";
                const string assetExt = "asset";
#if DEBUG_LOG
                builder.Append("Checking for changed assets");
#endif
                for (int i = 0; i < arrays.Length; i++)
                {
                    if (arrays[i] == null || arrays[i].Length <= 0)
                    {
#if DEBUG_LOG
                        builder.AppendLine().Append(i).Append("|- :: ").Append("IS NULL");
#endif
                        continue;
                    }
                    for (int j = 0; j < arrays[i].Length; j++)
                    {
#if DEBUG_LOG
                        builder.AppendLine().Append(i).Append('|').Append(j).Append(" :: ");
#endif
                        string value = arrays[i][j];
                        if (string.IsNullOrEmpty(value))
                        {
#if DEBUG_LOG
                            builder.Append("IS EMPTY");
#endif
                            continue;
                        }
#if DEBUG_LOG
                        builder.Append(value).Append(" - ");
#endif
                        string ext = Path.GetExtension(value);
                        bool match = ext.ToLower().Contains(assetExt);
                        if (!value.StartsWith(assetPref))
                        {
                            continue;
                        }
                        if (ignoredFiles.Contains(Path.GetFileNameWithoutExtension(value)))
                        {
                            continue;
                        }
                        if (match)
                        {
                            switch (i)
                            {
                                case 0:
                                case 2:
                                    if (alreadyLoaded == null)
                                    {
                                        alreadyLoaded = new HashSet<string>();
                                        IEnumerable<string> allLoaded =
                                            AssetDatabase.FindAssets("t:UniversalLoader")
                                            .Select(AssetDatabase.GUIDToAssetPath)
                                            .Select(AssetDatabase.LoadAssetAtPath<UniversalLoader>)
                                            .Select(l => new SerializedObject(l))
                                            .Select(o => o.FindProperty("objects"))
                                            .SelectMany(p => p.GetPropertiesInArray())
                                            .Select(a => a.objectReferenceValue)
                                            .Where(o => o != null)
                                            .Select(AssetDatabase.GetAssetPath);
                                        foreach (string loaded in allLoaded)
                                        {
                                            if (!alreadyLoaded.Contains(loaded))
                                            {
                                                alreadyLoaded.Add(loaded);
                                            }
                                        }
                                    }
                                    if (alreadyLoaded.Contains(value))
                                    {
#if DEBUG_LOG
                                        builder.Append("already loaded");
#endif
                                        continue;
                                    }
                                    break;
                            }
#if DEBUG_LOG
                            builder.Append("matches: \"").Append(ext).Append("\"");
#endif
                            return true;
                        }
                    }
                }
                return false;
            }
            finally
            {
#if DEBUG_LOG
                UnityEngine.Debug.Log(builder.ToString());
#endif
            }
        }
    }
#endif
}
