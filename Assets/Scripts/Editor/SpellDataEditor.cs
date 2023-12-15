using SummonsTracker.Characters;
using SummonsTracker.Text;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CustomEditor(typeof(SpellData), editorForChildClasses: true)]
    public class SpellDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            using (var licenseProperty = serializedObject.FindProperty("_license"))
            {
                EditorGUILayout.PropertyField(licenseProperty);
            }
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (target is SummonSpellData summonData)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("Test Spell");
                    _spellLevel = EditorGUILayout.IntSlider("Spell Level", _spellLevel, summonData.MinimumLevel, 9);
                    var spellParameters = summonData.GetSpellParameter(_spellLevel).ToArray();

                    var parameter = Mathf.Clamp(_spellParameter, 0, spellParameters.Length - 1);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel("Spell Parameter");
                        var enabled = GUI.enabled;

                        GUI.enabled = spellParameters.Any();

                        var pGUIContent = new GUIContent(_spellParameter.ToString());
                        EditorGUILayout.LabelField(pGUIContent, GUILayout.Width(GUI.skin.label.CalcSize(pGUIContent).x));
                        var rect = EditorGUILayout.GetControlRect();
                        rect.y += 1;
                        var spellParameterLabel = "None";
                        if (spellParameters.Any())
                        {
                            spellParameterLabel = spellParameters[parameter];
                            var slashIndex = spellParameterLabel.LastIndexOf('/');
                            if (slashIndex != -1)
                            {
                                spellParameterLabel = spellParameterLabel.Substring(slashIndex + 1);
                            }
                        }
                        if (GUI.Button(rect, spellParameterLabel, EditorStyles.miniPullDown))
                        {
                            var menu = new GenericMenu();
                            for (int i = 0; i < spellParameters.Length; i++)
                            {
                                int k = i;
                                menu.AddItem(new GUIContent(spellParameters[i]),
                                    k == parameter,
                                    () =>
                                    {
                                        _spellParameter = k;
                                        var num = summonData.GetNumberOfSummons(_spellLevel, _spellParameter, -1);
                                        ResetSpellParameterArray(num, summonData.GetPerSummonParameters(_spellLevel, _spellParameter).Count() - 1);
                                    });
                            }
                            menu.DropDown(rect);
                        }
                        GUI.enabled = enabled;
                    }
                    _maxNumber = EditorGUILayout.IntSlider("Summon", _maxNumber, -1, summonData.GetNumberOfSummons(_spellLevel, _spellParameter));
                    var numberOfSummons = summonData.GetNumberOfSummons(_spellLevel, _spellParameter, -1);
                    if (_maxNumber != -1)
                    {
                        numberOfSummons = Mathf.Min(numberOfSummons, _maxNumber);
                    }
                    var summonParameters = summonData.GetPerSummonParameters(_spellLevel, _spellParameter).ToArray();

                    if (numberOfSummons != _summonParameters.Length)
                    {
                        ResetSpellParameterArray(numberOfSummons, summonParameters.Length - 1);
                    }
                    for (int i = 0; i < numberOfSummons; i++)
                    {

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PrefixLabel($"Summon {i}");
                            var enabled = GUI.enabled;
                            GUI.enabled = summonParameters.Any();
                            var rect = EditorGUILayout.GetControlRect();
                            rect.y += 1;

                            var perSummonParameterLabel = "None";
                            if (summonParameters.Any())
                            {
                                var spellParameterClampI = Mathf.Clamp(i, 0, _summonParameters.Length - 1);
                                var summonParameterClampI = Mathf.Clamp(_summonParameters[spellParameterClampI], 0, summonParameters.Length - 1);
                                perSummonParameterLabel = summonParameters[summonParameterClampI];


                                var slashIndex = perSummonParameterLabel.LastIndexOf('/');
                                if (slashIndex != -1)
                                {
                                    perSummonParameterLabel = perSummonParameterLabel.Substring(slashIndex + 1);
                                }
                            }

                            if (GUI.Button(rect, perSummonParameterLabel, EditorStyles.miniPullDown))
                            {
                                var menu = new GenericMenu();
                                for (int j = 0; j < summonParameters.Length; j++)
                                {
                                    int ki = i;
                                    int kj = j;
                                    menu.AddItem(new GUIContent(summonParameters[j]),
                                        kj == _summonParameters[ki],
                                        () => _summonParameters[ki] = kj);
                                }
                                menu.DropDown(rect);
                            }
                            GUI.enabled = enabled;
                        }
                    }
                    if (GUILayout.Button("Summon"))
                    {
                        var c = summonData.GetCharacters(_spellLevel, _spellParameter, i => _summonParameters[i], _maxNumber);
                        _characters = c.ToArray();

                        wrapLabel = new GUIStyle(GUI.skin.label) { wordWrap = true, alignment = TextAnchor.UpperLeft, richText = true };
                    }
                }
                EditorGUILayout.Space();

                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    if (_characters == null || _characters.Length < 0)
                    {
                        EditorGUILayout.LabelField("No Summons");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Summons");
                        for (int i = 0; i < _characters.Length; i++)
                        {
                            using (new EditorGUI.IndentLevelScope(1))
                            {
                                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                                {
                                    EditorGUILayout.LabelField(string.IsNullOrEmpty(_characters[i].Name) ? "no name" : _characters[i].Name, EditorStyles.boldLabel);
                                    EditorGUILayout.LabelField(_characters[i].Creature.ToString(), new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Italic });

                                    EditorGUILayout.LabelField("Movement", string.Join(", ", _characters[i].Movement.Select(m => $"{m.Type} {m.Distance}ft.").ToArray()));

                                    EditorGUILayout.LabelField("Hit Points", _characters[i].MaxHP.ToString());
                                    EditorGUILayout.LabelField("Armor Class", _characters[i].AC.ToString());

                                    EditorGUILayout.LabelField("Proficiency", TextUtils.AddPlus(_characters[i].Proficiency));

                                    EditorGUILayout.PrefixLabel("Stats");
                                    var r = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 3);
                                    const int rows = 3;
                                    const int cols = 6;
                                    var statStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };

                                    EditorGUI.LabelField(GetSegment(r, 0, 0, cols, rows), "STR", statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 1, 0, cols, rows), "DEX", statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 2, 0, cols, rows), "CON", statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 3, 0, cols, rows), "INT", statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 4, 0, cols, rows), "WIS", statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 5, 0, cols, rows), "CHA", statStyle);

                                    EditorGUI.LabelField(GetSegment(r, 0, 1, cols, rows), _characters[i].Strength.ToString(), statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 1, 1, cols, rows), _characters[i].Dexterity.ToString(), statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 2, 1, cols, rows), _characters[i].Constitution.ToString(), statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 3, 1, cols, rows), _characters[i].Intelligence.ToString(), statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 4, 1, cols, rows), _characters[i].Wisdom.ToString(), statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 5, 1, cols, rows), _characters[i].Charisma.ToString(), statStyle);

                                    EditorGUI.LabelField(GetSegment(r, 0, 2, cols, rows), TextUtils.AddPlus(_characters[i].StrengthMod, true), statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 1, 2, cols, rows), TextUtils.AddPlus(_characters[i].DexterityMod, true), statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 2, 2, cols, rows), TextUtils.AddPlus(_characters[i].ConstitutionMod, true), statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 3, 2, cols, rows), TextUtils.AddPlus(_characters[i].IntelligenceMod, true), statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 4, 2, cols, rows), TextUtils.AddPlus(_characters[i].WisdomMod, true), statStyle);
                                    EditorGUI.LabelField(GetSegment(r, 5, 2, cols, rows), TextUtils.AddPlus(_characters[i].CharismaMod, true), statStyle);


                                    if (_characters[i].Skills != 0)
                                        EditorGUILayout.LabelField("Skills", _characters[i].Skills.ToString());
                                    if (_characters[i].SavingThrows != 0)
                                        EditorGUILayout.LabelField("Saving Throws", _characters[i].SavingThrows.ToString());
                                    if (_characters[i].ConditionImmunities != 0)
                                        EditorGUILayout.LabelField("Condition Immunities", _characters[i].ConditionImmunities.ToString());
                                    if (_characters[i].DamageVulnerabilities != 0)
                                        EditorGUILayout.LabelField("Damage Vulnerabilities", _characters[i].DamageVulnerabilities.ToString());
                                    if (_characters[i].DamageResistances != 0)
                                        EditorGUILayout.LabelField("Damage Resistances", _characters[i].DamageResistances.ToString());
                                    if (_characters[i].DamageImmunities != 0)
                                        EditorGUILayout.LabelField("Damage Immunities", _characters[i].DamageImmunities.ToString());

                                    EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
                                    for (int j = 0; j < _characters[i].Actions.Length; j++)
                                    {
                                        using var actionScope = new EditorGUILayout.HorizontalScope();
                                        var prefixWidth = EditorGUIUtility.labelWidth;
                                        var actionName = _characters[i].Actions[j].Name;
                                        EditorGUILayout.PrefixLabel(actionName);
                                        var desc = _characters[i].Actions[j].ToString();
                                        if (desc.StartsWith(actionName))
                                        {
                                            desc = desc.Substring(actionName.Length);
                                            while (!(char.IsLetter(desc[0]) || desc[0] == '<'))
                                            {
                                                desc = desc.Substring(1);
                                            }
                                        }
                                        var action = new GUIContent(desc.Trim());
                                        var actionRect = wrapLabel.CalcSize(action);
                                        var actionHeight = wrapLabel.CalcHeight(action, Screen.width - prefixWidth - 70);
                                        EditorGUILayout.LabelField(
                                            label: action,
                                            style: wrapLabel
                                            //options: GUILayout.Height(Mathf.CeilToInt(actionHeight / EditorGUIUtility.singleLineHeight) * EditorGUIUtility.singleLineHeight));
                                            );
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ResetSpellParameterArray(int numberOfSummons, int numberOfSummonparameters)
        {
            var newArr = new int[numberOfSummons];
            for (int i = 0; i < Mathf.Min(numberOfSummons, _summonParameters.Length); i++)
            {
                newArr[i] = Mathf.Clamp(_summonParameters[i], 0, numberOfSummonparameters);
            }
            _summonParameters = newArr;
        }

        private Rect GetSegment(Rect rect, int col, int row, int maxCols, int maxRows)
        {
            var width = rect.width / maxCols;
            var height = rect.height / maxRows;
            return new Rect(rect.x + (width * col), rect.y + (height * row), width, height);
        }

        private int _spellLevel;
        private int _maxNumber = -1;
        private int _spellParameter;
        private int[] _summonParameters = new int[0];

        private GUIStyle wrapLabel;

        private Character[] _characters;
    }
}