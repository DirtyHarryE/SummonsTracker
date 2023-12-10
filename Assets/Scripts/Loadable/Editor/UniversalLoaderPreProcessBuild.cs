using SummonsTracker.Loading;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace SummonsTracker.LoadingEditor
{
    public class UniversalLoaderPreProcessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            try
            {
                EditorUtility.DisplayProgressBar("Finding Loaders", "", 0);
                string[] guids = AssetDatabase.FindAssets("t:UniversalLoader");
                if (guids == null || guids.Length <= 0)
                {
                    ThrowError("Universal Loader Asset cannot be found!");
                    return;
                }

                bool foundSomethingToLoad = false;

                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                    EditorUtility.DisplayProgressBar("Checking Loaders", path, (i + 1f) / guids.Length);

                    UniversalLoader loader = AssetDatabase.LoadAssetAtPath<UniversalLoader>(path);
                    if (loader == null)
                    {
                        ThrowError("Universal Loader Asset cannot be found!");
                        return;
                    }

                    try
                    {
                        ILoadable[] loadables = loader.GetLoadables();
                        if (loadables.Length > 0)
                        {
                            foundSomethingToLoad = true;
                        }
                    }
                    catch (System.Exception e)
                    {
                        ThrowError("Error occured when loading UniversalLoader \"" + path + "\"\n" + e);
                        return;
                    }
                }
                if (!foundSomethingToLoad)
                {
                    ThrowError("All loadables have nothing to load. This could mean they are corrupt");
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        private void ThrowError(string errorMessage)
        {
            EditorUtility.DisplayDialog("Error", errorMessage, "Ok");
            throw new BuildFailedException(errorMessage);
        }
    }
}