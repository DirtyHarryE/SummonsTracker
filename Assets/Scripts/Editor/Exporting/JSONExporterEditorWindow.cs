using SummonsTracker.Licensing;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SummonsTracker.Exporter
{
    public class JSONExporterEditorWindow : EditorWindow
    {
        [MenuItem("Summoner/JSON/Export")]
        public static void ShowWindow()
        {
            var wnd = GetWindow(typeof(JSONExporterEditorWindow));
            wnd.titleContent = new GUIContent(EditorGUIUtility.IconContent("d_Text Icon")) { text = "JSON Exporter" };
        }

        public void OnGUI()
        {
            var enabled = GUI.enabled;
            GUI.enabled = !_finding;
            if (GUILayout.Button("Find Licesned"))
            {
                _loadingCoroutine = EditorCoroutine.StartCoroutine(FindLicensed());
            }

            using (var scrollScope = new GUILayout.ScrollViewScope(_scroll))
            {
                if (!_finding)
                {
                    if (_licensed != null)
                    {
                        foreach (var licensed in _licensed)
                        {
                            if (licensed is Object obj)
                            {
                                EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(obj.name), obj, typeof(Object), true);
                            }
                        }
                    }
                    _scroll = scrollScope.scrollPosition;
                }
            }
            GUI.enabled = enabled;
        }

        private IEnumerator FindLicensed()
        {
            _finding = true;

            var guids = AssetDatabase.FindAssets("t:Object");
            _licensed.Clear();
            var hash = new HashSet<string>();
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (path.EndsWith(".scene"))
                {
                    continue;
                }
                if (hash.Contains(path))
                {
                    continue;
                }
                else
                {
                    hash.Add(path);
                }
                if (EditorUtility.DisplayCancelableProgressBar("Find Licensed", path, ((float)i) / (float)(guids.Length)))
                {
                    //_loadingCoroutine.Stop();
                    break;
                }
                yield return null;
                var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                yield return null;
                foreach (var asset in assets)
                {
                    if (asset is ILicensing licensed)
                    {
                        _licensed.Add(licensed);
                    }
                    yield return null;
                }
            }
            EditorUtility.ClearProgressBar();
            _finding = false;
        }

        private bool _finding;
        private EditorCoroutine _loadingCoroutine;
        private List<ILicensing> _licensed = new List<ILicensing>();
        private Vector2 _scroll;
    }
}