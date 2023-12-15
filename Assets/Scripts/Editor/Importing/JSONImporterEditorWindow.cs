using SummonsTracker.Characters;
using SummonsTracker.EditorUtilities;
using SummonsTracker.Rolling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Importer
{
    public class JSONImporterEditorWindow : EditorWindow
    {
        [MenuItem("Summoner/JSON/Import")]
        public static void ShowWindow()
        {
            var wnd = GetWindow(typeof(JSONImporterEditorWindow));
            wnd.titleContent = new GUIContent(EditorGUIUtility.IconContent("d_Text Icon")) { text = "JSON Importer" };
        }

        public void OnGUI()
        {
            DrawHeader();
            DrawBody();
        }

        private void DrawHeader()
        {
            var pathPref = EditorPrefs.GetString(JSON_PATH, Application.dataPath);
            var savePref = EditorPrefs.GetString(SAVE_PATH, Application.dataPath);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Import", EditorStyles.boldLabel);
                using (new EditorGUILayout.HorizontalScope())
                {
                    pathPref = EditorGUILayout.TextField(pathPref);
                    if (GUILayout.Button("Open"))
                    {
                        pathPref = EditorUtility.OpenFilePanel("JSON", pathPref, "json");
                    }
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    savePref = EditorGUILayout.TextField(savePref);
                    if (GUILayout.Button("Save"))
                    {
                        savePref = EditorUtility.OpenFolderPanel("JSON Character", savePref, "Summons");
                    }
                }
                if (GUILayout.Button("Import"))
                {
                    InitStyles();
                    if (File.Exists(pathPref))
                    {
                        try
                        {
                            TryImport(pathPref);
                        }
                        catch (Exception e)
                        {
                            EditorUtility.DisplayDialog("JSON", $"An error occured!\n\n{e.Message}", "OK");
                        }
                        _currentCharacterData = null;
                        _currentCharacterEditor = null;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("JSON", "You must select a JSON first!", "OK");
                    }
                }
            }

            if (pathPref != EditorPrefs.GetString(JSON_PATH))
            {
                EditorPrefs.SetString(JSON_PATH, pathPref);
            }
            if (savePref != EditorPrefs.GetString(SAVE_PATH))
            {
                EditorPrefs.SetString(SAVE_PATH, savePref);
            }
        }

        private void DrawBody()
        {
            if (_monsters.Any())
            {
                using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                {
                    if (GUILayout.Button(EditorGUIUtility.IconContent("back"), GUILayout.MinHeight(EditorGUIUtility.singleLineHeight)))
                    {
                        _currentMonsterIndex -= 1;
                        _currentCharacterData = null;
                        _currentCharacterEditor = null;
                    }
                    var indexReadout = _currentMonsterIndex % _monsters.Length;
                    indexReadout = indexReadout < 0 ? _monsters.Length + indexReadout : indexReadout;
                    indexReadout += 1;
                    GUILayout.TextField($"{indexReadout} / {_monsters.Length}", _indexReadout);
                    if (GUILayout.Button(EditorGUIUtility.IconContent("forward"), GUILayout.MinHeight(EditorGUIUtility.singleLineHeight)))
                    {
                        _currentMonsterIndex += 1;
                        _currentCharacterData = null;
                        _currentCharacterEditor = null;
                    }
                }
                var index = _currentMonsterIndex % _monsters.Length;
                index = index < 0 ? _monsters.Length + index : index;
                DrawMonster(_monsters[index]);
            }
        }

        private void DrawMonster(Monster monster)
        {
            if (_currentCharacterEditor == null || _currentCharacterData == null)
            {
                using (var scroll = new EditorGUILayout.ScrollViewScope(_scroll))
                {
                    DrawMonsterRaw(monster);
                    GUILayout.FlexibleSpace();
                    _scroll = scroll.scrollPosition;
                }
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Save"))
                    {
                        _currentCharacterData = ScriptableObject.CreateInstance<CharacterData>();
                        _currentCharacterEditor = Editor.CreateEditor(_currentCharacterData) as CharacterDataEditor;
                        InitialiseCharacterData(monster, _currentCharacterData);
                    }
                }
            }
            else
            {
                using (var scroll = new EditorGUILayout.ScrollViewScope(_scroll))
                {
                    _currentCharacterEditor.OnInspectorGUI();
                    _scroll = scroll.scrollPosition;
                }
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Discard"))
                    {
                        _currentCharacterData = null;
                        _currentCharacterEditor = null;
                    }
                    if (GUILayout.Button("Save"))
                    {
                        Save(_currentCharacterData);
                    }
                }
            }
            if (GUILayout.Button("Save All"))
            {
                _coroutine = EditorCoroutine.StartCoroutine(SaveAll(), e => EditorUtility.ClearProgressBar());
            }
        }

        private void DrawMonsterRaw(Monster monster)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField("Name", monster.name, _htmlContentStyle);
                EditorGUILayout.LabelField("meta", monster.meta, _htmlContentStyle);

                EditorGUILayout.LabelField("AC", monster.ArmorClass, _htmlContentStyle);
                EditorGUILayout.LabelField("Hitpoints", monster.HitPoints, _htmlContentStyle);
                EditorGUILayout.LabelField("Speed", monster.Speed, _htmlContentStyle);


                EditorGUILayout.PrefixLabel("Stats");
                var r = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 3);
                const int rows = 3;
                const int cols = 6;

                EditorGUI.LabelField(GetSegment(r, 0, 0, cols, rows), "STR", _statStyle);
                EditorGUI.LabelField(GetSegment(r, 1, 0, cols, rows), "DEX", _statStyle);
                EditorGUI.LabelField(GetSegment(r, 2, 0, cols, rows), "CON", _statStyle);
                EditorGUI.LabelField(GetSegment(r, 3, 0, cols, rows), "INT", _statStyle);
                EditorGUI.LabelField(GetSegment(r, 4, 0, cols, rows), "WIS", _statStyle);
                EditorGUI.LabelField(GetSegment(r, 5, 0, cols, rows), "CHA", _statStyle);

                EditorGUI.LabelField(GetSegment(r, 0, 1, cols, rows), monster.STR, _statStyle);
                EditorGUI.LabelField(GetSegment(r, 1, 1, cols, rows), monster.DEX, _statStyle);
                EditorGUI.LabelField(GetSegment(r, 2, 1, cols, rows), monster.CON, _statStyle);
                EditorGUI.LabelField(GetSegment(r, 3, 1, cols, rows), monster.INT, _statStyle);
                EditorGUI.LabelField(GetSegment(r, 4, 1, cols, rows), monster.WIS, _statStyle);
                EditorGUI.LabelField(GetSegment(r, 5, 1, cols, rows), monster.CHA, _statStyle);

                EditorGUI.LabelField(GetSegment(r, 0, 2, cols, rows), monster.STR_mod, _statStyle);
                EditorGUI.LabelField(GetSegment(r, 1, 2, cols, rows), monster.DEX_mod, _statStyle);
                EditorGUI.LabelField(GetSegment(r, 2, 2, cols, rows), monster.CON_mod, _statStyle);
                EditorGUI.LabelField(GetSegment(r, 3, 2, cols, rows), monster.INT_mod, _statStyle);
                EditorGUI.LabelField(GetSegment(r, 4, 2, cols, rows), monster.WIS_mod, _statStyle);
                EditorGUI.LabelField(GetSegment(r, 5, 2, cols, rows), monster.CHA_mod, _statStyle);


                EditorGUILayout.LabelField("SavingThrows", monster.SavingThrows, _htmlContentStyle);
                EditorGUILayout.LabelField("Skills", monster.Skills, _htmlContentStyle);
                EditorGUILayout.LabelField("Senses", monster.Senses, _htmlContentStyle);
                EditorGUILayout.LabelField("Languages", monster.Languages, _htmlContentStyle);
                EditorGUILayout.LabelField("Challenge", monster.Challenge, _htmlContentStyle);

                HTMLField("Traits", monster.Traits);
                HTMLField("Actions", monster.Actions);

                HTMLField("LegendaryActions", monster.LegendaryActions);

                var urlRect = EditorGUILayout.GetControlRect();
                urlRect = EditorGUI.PrefixLabel(urlRect, new GUIContent("img_url"));
                EditorGUI.SelectableLabel(urlRect, monster.img_url);

                EditorGUILayout.LabelField("DamageImmunities", monster.DamageImmunities, _htmlContentStyle);

                EditorGUILayout.LabelField("ConditionImmunities", monster.ConditionImmunities, _htmlContentStyle);

                EditorGUILayout.LabelField("DamageResistances", monster.DamageResistances, _htmlContentStyle);

                EditorGUILayout.LabelField("DamageVulnerabilities", monster.DamageVulnerabilities, _htmlContentStyle);
                HTMLField("Reactions", monster.Reactions);
            }
        }

        private void InitialiseCharacterData(Monster monster, CharacterData characterData)
        {
            using var serializedObject = new SerializedObject(characterData);
            var monsterName = monster.name.Trim('.');

            var bOpenInd = monsterName.IndexOf('(');
            var bCloseInd = monsterName.IndexOf(')');

            if (bOpenInd != -1 && bCloseInd != -1)
            {
                monsterName = $"{monsterName.Substring(0, bOpenInd)}{monsterName.Substring(bCloseInd)}";
            }


            using (var nameSerializedProperty = serializedObject.FindProperty("_name"))
            {
                nameSerializedProperty.stringValue = monsterName;
            }
            using (var creatureSerializedProperty = serializedObject.FindProperty("_creature"))
            {
                SetCreatureEnum(creatureSerializedProperty, monster.meta);
            }
            using (var acSerializedProperty = serializedObject.FindProperty("_ac"))
            {
                SetArmorClass(acSerializedProperty, monster.ArmorClass);
            }
            using (var hpSerializedProperty = serializedObject.FindProperty("_maxHP"))
            {
                DiceUtility.FromString(monster.HitPoints, out int number, out int faces, out int modifiers);

                using var numberProperty = hpSerializedProperty.FindPropertyRelative("_number");
                using var facesProperty = hpSerializedProperty.FindPropertyRelative("_faces");
                using var modifiersProperty = hpSerializedProperty.FindPropertyRelative("_modifiers");

                numberProperty.intValue = number;
                facesProperty.intValue = faces;
                modifiersProperty.intValue = modifiers;
            }

            using (var movementSerializedProperty = serializedObject.FindProperty("_movement"))
            {
                SetMovement(movementSerializedProperty, monster.Speed);
            }


            var strength = 10;
            var dexterity = 10;
            var constitution = 10;
            var intelligence = 10;
            var wisdom = 10;
            var charisma = 10;

            if (int.TryParse(monster.STR, out var strParsed))
            {
                strength = strParsed;
            }
            if (int.TryParse(monster.DEX, out var dexParsed))
            {
                dexterity = dexParsed;
            }
            if (int.TryParse(monster.CON, out var conParsed))
            {
                constitution = conParsed;
            }
            if (int.TryParse(monster.INT, out var intParsed))
            {
                intelligence = intParsed;
            }
            if (int.TryParse(monster.WIS, out var wisParsed))
            {
                wisdom = wisParsed;
            }
            if (int.TryParse(monster.CHA, out var chaParsed))
            {
                charisma = chaParsed;
            }



            using (var strSerializedProperty = serializedObject.FindProperty("_strength"))
            {
                strSerializedProperty.intValue = strength;
            }
            using (var dexSerializedProperty = serializedObject.FindProperty("_dexterity"))
            {
                dexSerializedProperty.intValue = dexterity;
            }
            using (var conSerializedProperty = serializedObject.FindProperty("_constitution"))
            {
                conSerializedProperty.intValue = constitution;
            }
            using (var intSerializedProperty = serializedObject.FindProperty("_intelligence"))
            {
                intSerializedProperty.intValue = intelligence;
            }
            using (var wisSerializedProperty = serializedObject.FindProperty("_wisdom"))
            {
                wisSerializedProperty.intValue = wisdom;
            }
            using (var chaSerializedProperty = serializedObject.FindProperty("_charisma"))
            {
                chaSerializedProperty.intValue = charisma;
            }

            var proficiencyBonus = 2;
            using (var savingThrowSerializedProperty = serializedObject.FindProperty("_savingThrows"))
            {
                SetSavingThrows(savingThrowSerializedProperty, monster.SavingThrows, strength, dexterity, constitution, intelligence, wisdom, charisma, ref proficiencyBonus);
            }


            using (var skillsSerializedProperty = serializedObject.FindProperty("_skills"))
            {
                SetSkills(skillsSerializedProperty, monster.Skills);
            }
            using (var vulnerabilitiesSerializedProperty = serializedObject.FindProperty("_damageVulnerabilities"))
            {
                SetDamages(vulnerabilitiesSerializedProperty, monster.DamageVulnerabilities);
            }
            using (var resistancesSerializedProperty = serializedObject.FindProperty("_damageResistances"))
            {
                SetDamages(resistancesSerializedProperty, monster.DamageResistances);
            }
            using (var immunitiesSerializedProperty = serializedObject.FindProperty("_damageImmunities"))
            {
                SetDamages(immunitiesSerializedProperty, monster.DamageImmunities);
            }
            using (var immunitiesSerializedProperty = serializedObject.FindProperty("_conditionImmunities"))
            {
                SetConditions(immunitiesSerializedProperty, monster.ConditionImmunities);
            }
            using (var profSerializedProperty = serializedObject.FindProperty("_proficiency"))
            {
                profSerializedProperty.intValue = proficiencyBonus;
            }
            using (var sensesSerializedProperty = serializedObject.FindProperty("_senses"))
            {
                sensesSerializedProperty.stringValue = monster.Senses;
            }
            using (var languagesSerializedProperty = serializedObject.FindProperty("_languages"))
            {
                languagesSerializedProperty.stringValue = monster.Languages;
            }
            using (var challengeSerializedProperty = serializedObject.FindProperty("_challenge"))
            {
                challengeSerializedProperty.floatValue = ChallengeRatingHelper.CRToFloat(monster.Challenge);
            }
            using (var actionsSerializedProperty = serializedObject.FindProperty("_actions"))
            {
                SetActions(monsterName, actionsSerializedProperty, monster.Actions);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void SetCreatureEnum(SerializedProperty creatureSerializedProperty, string meta)
        {
            var metas = meta.Split(' ');
            var names = creatureSerializedProperty.enumNames;
            for (int i = 0; i < metas.Length; i++)
            {
                var m = metas[i].Trim(',').ToLower();
                for (int j = 0; j < names.Length; j++)
                {
                    if (m == names[j].ToLower())
                    {
                        creatureSerializedProperty.enumValueIndex = j;
                        return;
                    }
                }
            }
        }

        private void SetArmorClass(SerializedProperty acSerializedProperty, string armorClass)
        {
            var finalIndex = 1;
            for (int i = 0; i < armorClass.Length; i++)
            {
                var c = armorClass[i];
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }
                if (char.IsNumber(c))
                {
                    finalIndex = i + 1;
                }
                else
                {
                    break;
                }
            }
            var num = armorClass.Substring(0, finalIndex);
            acSerializedProperty.intValue = int.Parse(num.Trim());
        }

        private void SetMovement(SerializedProperty movementSerializedProperty, string movement)
        {
            if (string.IsNullOrEmpty(movement))
            {
                return;
            }
            var split = movement.Split(',');
            movementSerializedProperty.arraySize = split.Length;
            for (int i = 0; i < split.Length; i++)
            {
                var m = split[i].Trim().Split(' ');
                using var elementProperty = movementSerializedProperty.GetArrayElementAtIndex(i);
                for (int j = 0; j < m.Length; j++)
                {
                    if (int.TryParse(m[j], out var distance))
                    {
                        using var distanceProperty = elementProperty.FindPropertyRelative("Distance");
                        distanceProperty.intValue = distance;
                    }
                    else
                    {
                        using var typeProperty = elementProperty.FindPropertyRelative("Type");
                        for (int k = 0; k < typeProperty.enumNames.Length; k++)
                        {
                            if (m[j].Trim().ToLower() == typeProperty.enumNames[k].Trim().ToLower())
                            {
                                typeProperty.enumValueIndex = k;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void SetSavingThrows(SerializedProperty savingThrowSerializedProperty, string savingThrows, int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma, ref int proficiencyBonus)
        {
            if (string.IsNullOrEmpty(savingThrows))
            {
                return;
            }
            var split = savingThrows.Split(',');
            var curVal = 0;
            var list = new List<int>();
            for (int i = 0; i < split.Length; i++)
            {
                var statStr = split[i].Trim();
                var index = statStr.IndexOf('+');
                if (index == -1)
                {
                    index = statStr.IndexOf('-');
                }
                var statType = string.Empty;
                if (index != -1)
                {
                    statType = statStr.Substring(0, index).Trim();
                }
                var stat = GetStat(statType);
                curVal |= (int)stat;
                var modStr = statStr.Substring(index + 1);
                if (int.TryParse(modStr.Trim(), out var mod))
                {
                    var orig = stat switch
                    {
                        StatType.Strength => strength,
                        StatType.Dexterity => dexterity,
                        StatType.Constitution => constitution,
                        StatType.Intelligence => intelligence,
                        StatType.Wisdom => wisdom,
                        StatType.Charisma => charisma,
                        _ => 2,
                    };
                    list.Add(mod - CharacterData.GetMod(orig));
                }
            }
            savingThrowSerializedProperty.intValue = curVal;
            if (list.Any())
            {
                proficiencyBonus = Mathf.RoundToInt((float)list.Sum() / list.Count());
            }

            StatType GetStat(string s) => s switch
            {
                "STR" => StatType.Strength,
                "DEX" => StatType.Dexterity,
                "CON" => StatType.Constitution,
                "INT" => StatType.Intelligence,
                "WIS" => StatType.Wisdom,
                "CHA" => StatType.Charisma,
                _ => StatType.none,
            };
        }

        private void SetSkills(SerializedProperty skillsSerializedProperty, string skills)
        {
            if (string.IsNullOrEmpty(skills))
            {
                return;
            }
            var split = skills.Split(',');
            var curVal = 0;
            for (int i = 0; i < split.Length; i++)
            {
                var s = split[i].Trim();
                var index = s.IndexOf('+');
                if (index == -1)
                {
                    index = s.IndexOf('-');
                }
                if (index != -1)
                {
                    s = s.Substring(0, index).Trim();
                }
                for (int j = 1; j < skillsSerializedProperty.enumNames.Length; j++)
                {
                    var k = Mathf.FloorToInt(Mathf.Pow(2, j - 1));
                    var a = s.Replace(" ", "").ToLower();
                    var b = skillsSerializedProperty.enumNames[j].ToLower();
                    if (a == b)
                    {
                        curVal |= k;
                    }
                }
            }
            skillsSerializedProperty.intValue = curVal;
        }

        private void SetDamages(SerializedProperty immunitiesSerializedProperty, string damages)
        {
            if (string.IsNullOrEmpty(damages))
            {
                return;
            }
            var split = damages.Split(',');
            var curVal = 0;
            for (int i = 0; i < split.Length; i++)
            {
                for (int j = 1; j < immunitiesSerializedProperty.enumNames.Length; j++)
                {
                    var k = Mathf.FloorToInt(Mathf.Pow(2, j - 1));
                    var a = split[i].Replace(" ", "").ToLower();
                    var b = immunitiesSerializedProperty.enumNames[j].ToLower();
                    if (a == b)
                    {
                        curVal |= k;
                    }
                }
            }
            immunitiesSerializedProperty.intValue = curVal;
        }

        private void SetConditions(SerializedProperty immunitiesSerializedProperty, string conditions)
        {
            if (string.IsNullOrEmpty(conditions))
            {
                return;
            }
            var split = conditions.Split(',');
            var curVal = 0;
            for (int i = 0; i < split.Length; i++)
            {
                for (int j = 1; j < immunitiesSerializedProperty.enumNames.Length; j++)
                {
                    var k = Mathf.FloorToInt(Mathf.Pow(2, j - 1));
                    var a = split[i].Replace(" ", "").ToLower();
                    var b = immunitiesSerializedProperty.enumNames[j].ToLower();
                    if (a == b)
                    {
                        curVal |= k;
                    }
                }
            }
            immunitiesSerializedProperty.intValue = curVal;
        }

        private void SetActions(string characterName, SerializedProperty actionsSerializedProperty, string actions)
        {
            var rawEntries = GetEntries(actions, false);
            var entries = new List<(string, string)>();
            var runningBody = string.Empty;

            foreach (var entry in rawEntries)
            {
                var body = entry.Item2;
                if (string.IsNullOrEmpty(entry.Item1))
                {
                    runningBody = string.IsNullOrEmpty(runningBody) ? body : $"{runningBody}\n{body}";
                }
                else
                {
                    entries.Add((entry.Item1, string.IsNullOrEmpty(runningBody) ? body : runningBody));
                    runningBody = string.Empty;
                }
            }

            actionsSerializedProperty.arraySize = entries.Count;
            MultiattackData multiattackData = null;
            var hasMultiAttack = false;
            for (int i = 0; i < entries.Count; i++)
            {
                UnityEngine.Object data;
                if (AttackDataParser.TryMakeAttack(actionsSerializedProperty, i, entries[i].Item1, entries[i].Item2, out var attackData))
                {
                    data = attackData;
                }
                else if (SavingThrowParser.TryMakeSavingThrow(actionsSerializedProperty, i, entries[i].Item1, entries[i].Item2, out var savingThrowData))
                {
                    data = savingThrowData;
                }
                else if (MultiattackDataParser.TryMakeMultiattack(actionsSerializedProperty, i, entries[i].Item1, entries[i].Item2, out var m))
                {
                    data = m;
                    multiattackData = m;
                    hasMultiAttack = true;
                }
                else if (ActionDataParser.TryMakeAction(actionsSerializedProperty, i, entries[i].Item1, entries[i].Item2, out var actionData))
                {
                    data = actionData;
                }
                else
                {
                    continue;
                }
                var actionName = entries[i].Item1;

                var bOpenInd = actionName.IndexOf('(');
                var bCloseInd = actionName.IndexOf(')');

                if (bOpenInd != -1 && bCloseInd != -1)
                {
                    actionName = $"{actionName.Substring(0, bOpenInd)}{actionName.Substring(bCloseInd + 1)}";
                }

                data.name = actionName.Trim(' ', '.', ',');
                using (var elementProperty = actionsSerializedProperty.GetArrayElementAtIndex(i))
                {
                    elementProperty.objectReferenceValue = data;
                }
            }
            if (hasMultiAttack)
            {
                MultiattackDataParser.FillMultiattack(multiattackData, characterName, actionsSerializedProperty);
            }
        }

        private void HTMLField(string prefix, string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return;
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(prefix);
                if (!string.IsNullOrEmpty(html))
                {
                    var entries = GetEntries(html, false);
                    using (var vert = new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
                    {
                        foreach (var entry in entries)
                        {
                            var title = entry.Item1.Trim();
                            var explanation = entry.Item2.Trim().Replace("<em>", "<b>").Replace("</em>", "</b>");
                            if (!string.IsNullOrEmpty(title))
                            {
                                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                            }

                            if (!string.IsNullOrEmpty(explanation))
                            {
                                var e = _htmlContentStyle.CalcHeight(new GUIContent(explanation), Screen.width - EditorGUIUtility.labelWidth - 20);
                                EditorGUILayout.SelectableLabel(explanation, _htmlContentStyle, GUILayout.Height(e));
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<(string, string)> GetEntries(string html, bool removeHTMLTags = false)
        {
            if (!string.IsNullOrEmpty(html))
            {
                var strongOpens = new[] { "<em><strong>", "<strong>" };
                var strongCloses = new[] { "</strong></em>", "</strong>" };

                var h = html;
                for (int e = 0; e < 1000; e++)
                {
                    var oIndex = IndexOf(h, strongOpens, out var o);
                    var cIndex = IndexOf(h, strongCloses, out var c);
                    if (oIndex == -1 || cIndex == -1)
                    {
                        break;
                    }

                    var title = h.Substring(oIndex + o.Length, cIndex - oIndex - o.Length);
                    var endIndex = cIndex + c.Length;

                    var b = h.Substring(endIndex);
                    var nextOIndex = IndexOf(b, strongOpens, out var nextO);
                    if (nextOIndex == -1)
                    {
                        var finalBody = ReplaceParagraphs(b, removeHTMLTags);
                        yield return (title, finalBody);
                        break;
                    }
                    var body = ReplaceParagraphs(b.Substring(0, nextOIndex), removeHTMLTags);
                    yield return (title, body);
                    h = b.Substring(nextOIndex);
                }
            }
        }

        private int IndexOf(string text, string[] strings, out string foundString)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                var index = text.IndexOf(strings[i]);
                if (index != -1)
                {
                    foundString = strings[i];
                    return index;
                }
            }
            foundString = string.Empty;
            return -1;
        }

        private string ReplaceParagraphs(string text, bool removeHTMLTags)
        {
            var body = string.Join("\n", text.Trim().Split(new[] { "<p>", "</p>" }, StringSplitOptions.RemoveEmptyEntries));
            if (removeHTMLTags)
            {
                body = Regex.Replace(body.Trim(), "<.*?>", string.Empty); ;
            }
            return body;
        }

        private Rect GetSegment(Rect rect, int col, int row, int maxCols, int maxRows)
        {
            var width = rect.width / maxCols;
            var height = rect.height / maxRows;
            return new Rect(rect.x + (width * col), rect.y + (height * row), width, height);
        }

        private void TryImport(string path)
        {
            var json = File.ReadAllText(path);

            var monsters = JsonConvert.DeserializeObject<List<Monster>>(json);

            _monsters = monsters.ToArray();
        }

        private void InitStyles()
        {
            _statStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            _htmlContentStyle = new GUIStyle(EditorStyles.label) { wordWrap = true, richText = true };
            _indexReadout = new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleCenter };
        }

        private IEnumerator SaveAll()
        {
            for (int i = 0; i < _monsters.Length; i++)
            {
                if (_monsters[i].name.ToLower().Contains("wight"))
                {
                    continue;
                }
                var characterData = ScriptableObject.CreateInstance<CharacterData>();
                yield return new WaitForSeconds(0.01f);
                InitialiseCharacterData(_monsters[i], characterData);
                Save(characterData);
                if (_coroutine != null)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Importing", _monsters[i].name, ((float)i) / ((float)_monsters.Length)))
                    {
                        _coroutine.Stop();
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private void RemoveAllActions(SerializedProperty serializedProperty, ref List<UnityEngine.Object> actions)
        {
            if (serializedProperty == null)
            {
                return;
            }
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                using var elementProperty = serializedProperty.GetArrayElementAtIndex(i);
                if (elementProperty.objectReferenceValue != null)
                {
                    actions.Add(elementProperty.objectReferenceValue);
                }
            }
            serializedProperty.arraySize = 0;
        }

        private void Save(CharacterData characterData)
        {
            var savePref = EditorPrefs.GetString(SAVE_PATH, Application.dataPath).Trim();
            var assetIndex = savePref.LastIndexOf("Assets");
            if (assetIndex != -1)
            {
                savePref = savePref.Substring(assetIndex);
            }
            var pathName = $"{savePref}/{ValidFilename(characterData.Name)}.asset";
            Debug.Log(pathName);

            var existingAsset = AssetDatabase.LoadAssetAtPath<CharacterData>(pathName);
            if (existingAsset == null)
            {
                AssetDatabase.CreateAsset(characterData, pathName);
                for (int i = 0; i < characterData.Actions.Length; i++)
                {
                    AssetDatabase.AddObjectToAsset(characterData.Actions[i], characterData);
                }
                AssetDatabase.ImportAsset(pathName);
            }
            else
            {
                var existingSubAssets = AssetDatabase.LoadAllAssetsAtPath(pathName).Where(s => s != existingAsset);
                var serializedObjFrom = new SerializedObject(characterData);
                var serializedObjTo = new SerializedObject(existingAsset);

                foreach (var subAsset in existingSubAssets)
                {
                    AssetDatabase.RemoveObjectFromAsset(subAsset);
                }
                for (int i = 0; i < characterData.Actions.Length; i++)
                {
                    AssetDatabase.AddObjectToAsset(characterData.Actions[i], existingAsset);
                }
                SerializedObjectHelper.Copy(serializedObjFrom, serializedObjTo);
                AssetDatabase.ImportAsset(pathName);
            }
        }

        private string ValidFilename(string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        private GUIStyle _indexReadout;
        private GUIStyle _statStyle;
        private GUIStyle _htmlContentStyle;
        private Monster[] _monsters = Array.Empty<Monster>();
        private int _currentMonsterIndex;
        private Vector2 _scroll;
        private CharacterData _currentCharacterData;
        private CharacterDataEditor _currentCharacterEditor;
        private EditorCoroutine _coroutine;
        private const string JSON_PATH = "json_path";
        private const string SAVE_PATH = "save_path";
    }
}