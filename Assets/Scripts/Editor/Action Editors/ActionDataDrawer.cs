using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public abstract class ActionDrawer
    {
        public Rect BaseRect => _baseRect;
        public abstract ActionData Target { get; }

        public ActionDrawer(SerializedObject serializedObject)
        {
            SerializedObject = serializedObject;
            _newName = serializedObject.targetObject.name;
            _renameSkin = new GUIStyle(GUI.skin.label);
        }

        public void Draw(Rect position)
        {
            _baseRect = position;
            _runningHeight = 0;
            Indent = 0;
            OnDraw();
            SerializedObject.ApplyModifiedProperties();
        }

        public virtual float GetHeight()
        {
            return Mathf.Max(_runningHeight - 2 + MARGIN, EditorGUIUtility.singleLineHeight);
        }

        protected virtual void OnDraw()
        {
            DrawName();
            OnBeforeDrawNote();
            DrawNote();
        }

        protected virtual void OnBeforeDrawNote() { }

        protected Rect GetRect() => GetRect(EditorGUIUtility.singleLineHeight);
        protected Rect GetRect(float height)
        {
            var ind = Mathf.Clamp(Indent, 0, _baseRect.width);
            var m = Mathf.Approximately(_runningHeight, 0f) ? 1f : MARGIN;
            var r = new Rect(x: _baseRect.x + ind,
                             y: m + _baseRect.y + _runningHeight,
                             width: _baseRect.width - ind,
                             height: height);
            _runningHeight += height + m;
            return r;
        }

        public SerializedObject SerializedObject { get; private set; }
        protected float Indent = 0;

        protected void DrawName()
        {
            var prefixWidth = BaseRect.width * 0.3f;
            var button = _renaming ? _apply : _rename;
            var renameSize = _renameSkin.CalcSize(button);
            var mult = EditorGUIUtility.singleLineHeight / renameSize.y;
            renameSize = renameSize * mult;
            var lRect = new Rect(BaseRect.x, BaseRect.y + 1, prefixWidth - renameSize.x - 4, EditorGUIUtility.singleLineHeight);
            var rRect = new Rect(BaseRect.x + prefixWidth - renameSize.x - 2, BaseRect.y, renameSize.x, renameSize.y);
            if (_renaming)
            {
                _newName = EditorGUI.TextField(lRect, _newName);
            }
            else
            {
                EditorGUI.LabelField(lRect, _newName);
            }
            if (GUI.Button(rRect, button, _renameSkin))
            {
                if (_renaming)
                {
                    var newName = _newName.Trim();
                    if (Target.name != newName)
                    {
                        var invalid = Path.GetInvalidFileNameChars();
                        if (newName.IndexOfAny(invalid) == -1)
                        {
                            Target.name = newName;
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Target));
                            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                            _renaming = false;
                        }
                        else
                        {
                            var invalidStr = string.Join(", ", invalid.Where(c => !char.IsWhiteSpace(c) && !char.IsControl(c)).Select(c => c.ToString()).ToArray());
                            EditorUtility.DisplayDialog("Error", $"New name \"{newName}\" cannot contain any of the following\n\n{invalidStr}\n\nPlease give your action a name the doesn't contain any of these.", "OK");
                        }
                    }
                    else
                    {
                        _renaming = false;
                    }
                }
                else
                {
                    _renaming = true;
                }
            }

            Indent = prefixWidth;
        }

        protected void DrawNote()
        {
            using (var noteProp = SerializedObject.FindProperty("_note"))
            {
                var rect = GetRect(NoteSize);
                var w = rect.width - 13;
                var h = EditorStyles.textArea.CalcHeight(new GUIContent(noteProp.stringValue), w);
                var needScrollbar = rect.height < h;

                var view = needScrollbar ? new Rect(rect.x, rect.y, w, h) : rect;

                if (needScrollbar)
                {
                    _scroll = GUI.BeginScrollView(rect, _scroll, view, false, false);
                }
                noteProp.stringValue = EditorGUI.TextArea(view, noteProp.stringValue, EditorStyles.textArea);
                if (needScrollbar)
                {
                    GUI.EndScrollView();
                }
            }
        }

        protected virtual float NoteSize => EditorGUIUtility.singleLineHeight * 3;

        private Vector2 _scroll;

        private const float MARGIN = 2;
        private Rect _baseRect;
        private float _runningHeight;

        private bool _renaming;
        private string _newName;
        private static GUIContent _rename = EditorGUIUtility.IconContent("d_InputField Icon");
        private static GUIContent _apply = EditorGUIUtility.IconContent("FilterSelectedOnly");
        private static GUIStyle _renameSkin;
    }
    public class ActionDrawer<T> : ActionDrawer where T : ActionData
    {
        public override ActionData Target => _target;
        public ActionDrawer(T target) : base(new SerializedObject(target))
        {
            _target = target;
        }
        private T _target;
    }

    public class ActionDataDrawer : ActionDrawer<ActionData>
    {
        public ActionDataDrawer(ActionData target) : base(target) { }
    }
}