using SummonsTracker.Characters;
using SummonsTracker.EditorUtilities;
using SummonsTracker.Licensing;
using SummonsTracker.Rolling;
using SummonsTracker.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private void OnEnable()
        {

            var count = EditorPrefs.GetInt($"LicensedCount");
            for (int i = 0; i < count; i++)
            {
                var path = EditorPrefs.GetString($"Licensed{i}");
                if (!string.IsNullOrEmpty(path))
                {
                    var allAtPath = AssetDatabase.LoadAllAssetsAtPath(path);
                    foreach (var asset in allAtPath)
                    {
                        if (asset is ILicensing licensed)
                        {
                            _licensed.Add(licensed);
                        }
                    }
                }
            }
        }

        public void OnGUI()
        {
            var enabled = GUI.enabled;
            GUI.enabled = !_finding;
            if (GUILayout.Button("Find Licesned"))
            {
                EditorCoroutine.StartCoroutine(FindLicensed());
            }

            var hasString = _stringBuilder != null && !string.IsNullOrEmpty(_stringBuilder.ToString());

            var guiLayouts = hasString ? new[] { GUILayout.Height(Screen.height * 0.45f), GUILayout.MinHeight(Screen.height * 0.4f), GUILayout.MaxWidth(Screen.height * 0.5f) }
            : new[] { GUILayout.ExpandHeight(true) };

            using (new GUILayout.VerticalScope(guiLayouts))
            {
                using var scrollScope = new GUILayout.ScrollViewScope(_licensedScroll);
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
                _licensedScroll = scrollScope.scrollPosition;
            }
            GUI.enabled = !_finding && _licensed.Any();
            if (GUILayout.Button("Print"))
            {
                EditorCoroutine.StartCoroutine(BuildString());
            }
            if (hasString)
            {
                var built = _stringBuilder.ToString();

                var builtContent = new GUIContent(built);
                var guiStyle = new GUIStyle(EditorStyles.label) { wordWrap = true };
                var h = guiStyle.CalcHeight(builtContent, Screen.width);
                using (new GUILayout.VerticalScope(EditorStyles.textArea, guiLayouts))
                {
                    using var scroll = new EditorGUILayout.ScrollViewScope(_builderScroll);
                    EditorGUILayout.SelectableLabel(built, guiStyle, GUILayout.MinHeight(h), GUILayout.ExpandHeight(true));
                    _builderScroll = scroll.scrollPosition;
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
            GUI.enabled = enabled;
        }

        private IEnumerator FindLicensed()
        {
            _finding = true;

            var guids = AssetDatabase.FindAssets("t:Object", new[] { "Assets/" });
            _licensed.Clear();
            var hash = new HashSet<string>();
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var ext = Path.GetExtension(path).ToLower();
                if (ext == ".unity" || ext == ".scene")
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
            EditorPrefs.SetInt($"LicensedCount", _licensed.Count);
            for (int i = 0; i < _licensed.Count; i++)
            {
                if (_licensed[i] is Object obj)
                {
                    EditorPrefs.SetString($"Licensed{i}", AssetDatabase.GetAssetPath(obj));
                }
                else
                {
                    EditorPrefs.SetString($"Licensed{i}", string.Empty);
                }
            }
            _finding = false;
            Repaint();
        }

        private IEnumerator BuildString()
        {
            _finding = true;
            _stringBuilder = new StringBuilder();
            for (int i = 0; i < _licensed.Count; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Find Licensed", $"Exporting {i} / {_licensed.Count}", ((float)i) / (float)(_licensed.Count)))
                {
                    break;
                }
                yield return BuildString(_licensed[i], _stringBuilder, 0);
                _stringBuilder.AppendLine();
                yield return null;
                _stringBuilder.AppendLine();
            }
            EditorUtility.ClearProgressBar();
            Debug.Log(_stringBuilder.ToString());
            _finding = false;
            Repaint();
        }

        private IEnumerator BuildString(ILicensing licensed, StringBuilder builder, int indent)
        {
            if (licensed is Object obj)
            {
                yield return BuildString(obj, builder, indent);
                yield return null;
            }
        }

        private IEnumerator BuildString(Object obj, StringBuilder builder, int indent)
        {
            var indentStr = new string(' ', indent * 4);
            builder.Append(indentStr).Append(ObjectNames.NicifyVariableName(obj.GetType().Name).ToUpper()).AppendLine();
            builder.Append(indentStr).Append(ObjectNames.NicifyVariableName(obj.name));
            using var serializedObject = new SerializedObject(obj);
            using var iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);
            bool enterChildren;
            do
            {
                enterChildren = true;
                using var property = serializedObject.FindProperty(iterator.propertyPath);

                indentStr = new string(' ', indent * 4);
                var pathSplit = property.propertyPath.Split('.');
                var subIndent = pathSplit.Count(p => p != "Array");
                var subIndentStr = new string(' ', subIndent * 4);

                if (ShouldSkipProperty(property, ref enterChildren))
                {
                    continue;
                }

                if (IsMovementArray(property))
                {
                    builder.AppendLine().Append(indentStr).Append(subIndentStr).Append(property.displayName).Append(": ");
                    builder.Append(GetMovementArray(property));
                    enterChildren = false;
                }
                else if (property.isArray && property.propertyType != SerializedPropertyType.String)
                {
                    builder.AppendLine().Append(indentStr).Append(subIndentStr).Append(property.displayName).Append(": ");
                    enterChildren = true;
                }
                else if (property.propertyType == SerializedPropertyType.ArraySize)
                {
                    enterChildren = false;
                }
                else
                {
                    if (ParentIsArray(property))
                    {
                        if (property.propertyType != SerializedPropertyType.Generic)
                        {

                            builder.AppendLine().Append(indentStr).Append(subIndentStr);
                        }
                    }
                    else
                    {
                        builder.AppendLine().Append(indentStr).Append(subIndentStr);
                        builder.Append(property.displayName).Append(": ");
                    }
                    enterChildren = false;
                    switch (property.name)
                    {
                        case "_challenge":
                        {
                            builder.Append(ChallengeRatingHelper.FloatToCR(property.floatValue, true));
                            break;
                        }
                        case "_concentration":
                        {
                            builder.Append(property.boolValue ? "Yes" : "No");
                            break;
                        }
                        case "_proficiency":
                        case "_strength":
                        case "_dexterity":
                        case "_constitution":
                        case "_intelligence":
                        case "_wisdom":
                        case "_charisma":
                        {
                            builder.Append(property.intValue).Append(" ");
                            builder.Append("(").Append(TextUtils.AddPlus(CharacterHelper.GetMod(property.intValue))).Append(")");
                            break;
                        }
                        default:
                            switch (property.propertyType)
                            {
                                case SerializedPropertyType.ObjectReference:
                                    if (property.objectReferenceValue is ILicensing licensed)
                                    {
                                        builder.Append(property.objectReferenceValue.name);
                                    }
                                    else if (property.objectReferenceValue is MultiattackData multiattackData)
                                    {
                                        builder.Append(GetMultiattackString(multiattackData, serializedObject));
                                    }
                                    else if (property.objectReferenceValue is ActionData actionData)
                                    {
                                        builder.Append(GetActionString(actionData));
                                    }
                                    else
                                    {
                                        builder.AppendLine();
                                        yield return BuildString(property.objectReferenceValue, builder, indent + subIndent + 1);
                                    }
                                    break;
                                case SerializedPropertyType.Integer:
                                    builder.Append(property.intValue);
                                    break;
                                case SerializedPropertyType.Boolean:
                                    builder.Append(property.boolValue);
                                    break;
                                case SerializedPropertyType.Float:
                                    builder.Append(property.floatValue);
                                    break;
                                case SerializedPropertyType.String:
                                    builder.Append(property.stringValue);
                                    break;
                                case SerializedPropertyType.Color:
                                    builder.Append(property.colorValue);
                                    break;
                                case SerializedPropertyType.Enum:
                                    builder.Append(ObjectNames.NicifyVariableName(property.GetValueWithReflection().ToString()));
                                    break;
                                case SerializedPropertyType.Vector2:
                                    builder.Append(property.vector2Value);
                                    break;
                                case SerializedPropertyType.Vector3:
                                    builder.Append(property.vector3Value);
                                    break;
                                case SerializedPropertyType.Vector4:
                                    builder.Append(property.vector4Value);
                                    break;
                                case SerializedPropertyType.Rect:
                                    builder.Append(property.rectValue);
                                    break;
                                case SerializedPropertyType.Character:
                                    builder.Append(property.stringValue);
                                    break;
                                case SerializedPropertyType.AnimationCurve:
                                    builder.Append(property.animationCurveValue);
                                    break;
                                case SerializedPropertyType.Bounds:
                                    builder.Append(property.boundsValue);
                                    break;
                                case SerializedPropertyType.Quaternion:
                                    builder.Append(property.quaternionValue);
                                    break;
                                case SerializedPropertyType.Vector2Int:
                                    builder.Append(property.vector2IntValue);
                                    break;
                                case SerializedPropertyType.Vector3Int:
                                    builder.Append(property.vector3IntValue);
                                    break;
                                case SerializedPropertyType.RectInt:
                                    builder.Append(property.rectIntValue);
                                    break;
                                case SerializedPropertyType.BoundsInt:
                                    builder.Append(property.boundsIntValue);
                                    break;
                                case SerializedPropertyType.Generic:
                                    switch (GetPropertyTypeName(property.type))
                                    {
                                        case "Dice":
                                            builder.Append(GetDice(property));
                                            break;
                                        default:
                                            enterChildren = true;
                                            break;
                                    }
                                    break;
                                default:
                                    enterChildren = true;
                                    break;
                            }
                            break;
                    }
                }
                yield return null;
            } while (iterator.NextVisible(enterChildren));
        }

        private string GetMovementArray(SerializedProperty property)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < property.arraySize; i++)
            {
                if (i != 0)
                {
                    builder.Append(", ");
                }
                using var element = property.GetArrayElementAtIndex(i);
                using var m = element.FindPropertyRelative("Type");
                using var d = element.FindPropertyRelative("Distance");
                if (m.enumNames[m.enumValueIndex] != "Walk")
                {
                    builder.Append(m.enumDisplayNames[m.enumValueIndex]).Append(" ");
                }
                builder.Append(d.intValue).Append(" ft.");
            }
            return builder.ToString();
        }

        private bool IsMovementArray(SerializedProperty property)
        {
            if (!property.isArray)
            {
                return false;
            }
            var arrayType = GetPropertyTypeName(property.arrayElementType);
            if (arrayType == "Movement")
            {
                return true;
            }
            return false;
        }

        private bool ParentIsArray(SerializedProperty property)
        {
            var split = property.propertyPath.Split('.');

            var b = new StringBuilder();
            b.Append(split[0]);
            for (int i = 1; i < split.Length - 1; i++)
            {
                b.Append(".").Append(split[i]);
            }

            using var parent = property.serializedObject.FindProperty(b.ToString());
            if (parent != null && parent.isArray)
            {
                return true;
            }

            return false;
        }

        private bool ShouldSkipProperty(SerializedProperty property, ref bool enterChildren)
        {
            if (property.name.ToLower() == "m_script")
            {
                enterChildren = false;
                return true;
            }
            if (property.propertyType == SerializedPropertyType.String && string.IsNullOrWhiteSpace(property.stringValue))
            {
                enterChildren = false;
                return true;
            }
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                var enums = property.enumNames;
                if (0 <= property.intValue && property.intValue < enums.Length)
                {
                    var enumVal = enums[property.intValue];
                    if (enumVal.ToLower().Trim() == "none")
                    {
                        enterChildren = false;
                        return true;
                    }
                }
                if (0 <= property.enumValueIndex && property.enumValueIndex < enums.Length)
                {
                    var enumVal = enums[property.enumValueIndex];
                    if (enumVal.ToLower().Trim() == "none")
                    {
                        enterChildren = false;
                        return true;
                    }
                }
            }
            return false;
        }

        private string GetActionString(ActionData actionData)
        {
            var b = new StringBuilder();
            var action = actionData.Instantiate();
            var str = action.ToString();


            return Regex.Replace(str.Trim(), "<.*?>", string.Empty);
        }

        private string GetMultiattackString(MultiattackData multiattackData, SerializedObject serializedObject)
        {
            using var nameSerializedProperty = serializedObject.FindProperty("_name");
            using var multiattackSerializedObject = new SerializedObject(multiattackData);
            using var actionsProperty = serializedObject.FindProperty("_actions");

            using var multiAttacksProperty = multiattackSerializedObject.FindProperty("_attacks");

            var b = new StringBuilder();
            var count = 0;
            var nonAnyCount = 0;
            for (int i = 0; i < multiAttacksProperty.arraySize; i++)
            {
                using var attackElement = multiAttacksProperty.GetArrayElementAtIndex(i);
                using var attackNumber = attackElement.FindPropertyRelative("_attackNumber");
                count += attackNumber.intValue;
                using var attackIndex = attackElement.FindPropertyRelative("_attackIndex");
                if (attackIndex.intValue != -1)
                {
                    nonAnyCount += attackNumber.intValue;
                }
            }
            b.Append("The ").Append(nameSerializedProperty.stringValue).Append(" makes ").Append(count).Append(" attacks");
            if (nonAnyCount <= 0)
            {
                b.Append(".");
            }
            else
            {
                b.Append(": ");
                for (int i = 0; i < multiAttacksProperty.arraySize; i++)
                {
                    using var attackElement = multiAttacksProperty.GetArrayElementAtIndex(i);

                    using var attackIndex = attackElement.FindPropertyRelative("_attackIndex");
                    using var attackNumber = attackElement.FindPropertyRelative("_attackNumber");

                    if (attackIndex.intValue == -1)
                    {
                        continue;
                    }
                    using var attackProperty = actionsProperty.GetArrayElementAtIndex(attackIndex.intValue);
                    b.Append(attackNumber.intValue).Append(" ").Append(attackProperty.objectReferenceValue.name);
                    if (i <= multiAttacksProperty.arraySize - 3)
                    {
                        b.Append(", ");
                    }
                    else if (i == multiAttacksProperty.arraySize - 2)
                    {
                        b.Append(" and ");
                    }
                    else if (i == multiAttacksProperty.arraySize - 1)
                    {
                        b.Append(".");
                    }
                }
            }
            return b.ToString().Trim();
        }

        private string GetDice(SerializedProperty property)
        {
            using var numProp = property.FindPropertyRelative("_number");
            using var facesProp = property.FindPropertyRelative("_faces");
            using var modProp = property.FindPropertyRelative("_modifiers");

            if (numProp == null || facesProp == null || modProp == null)
            {
                return $"ERROR :: {property.displayName} - {property.propertyPath}";
            }

            var avg = DiceUtility.Average(numProp.intValue, facesProp.intValue, modProp.intValue);

            return facesProp.intValue == 0 || numProp.intValue == 0 ? modProp.intValue.ToString()
                : modProp.intValue > 0 ? $"{avg} ({numProp.intValue}d{facesProp.intValue}+{modProp.intValue})"
                : modProp.intValue < 0 ? $"{avg} ({numProp.intValue}d{facesProp.intValue}{modProp.intValue})"
                : $"{avg} ({numProp.intValue}d{facesProp.intValue})";
        }

        private string GetPropertyTypeName(string type)
        {
            const string prefix = "PPtr<$";
            const string suffix = ">";
            var indStart = type.IndexOf(prefix);
            var indEnd = type.LastIndexOf(suffix);
            if (indStart != -1 && indEnd != -1)
            {
                type = type.Substring(indStart + prefix.Length, indEnd - indStart - prefix.Length);
            }

            return type;
        }

        private StringBuilder _stringBuilder;

        private bool _finding;
        private List<ILicensing> _licensed = new List<ILicensing>();
        private Vector2 _licensedScroll;
        private Vector2 _builderScroll;
    }
}