using SummonsTracker.Characters;
using SummonsTracker.Rolling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.Importer
{
    public class JSONImporterEditorWindow : EditorWindow
    {
        [MenuItem("Summoner/JSON Importer")]
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
                    _scroll = scroll.scrollPosition;
                }
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Save"))
                    {
                        _currentCharacterData = ScriptableObject.CreateInstance<CharacterData>();
                        _currentCharacterEditor = Editor.CreateEditor(_currentCharacterData) as CharacterDataEditor;
                        InitialiseCharacterData(monster);
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
                }
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

                EditorGUILayout.LabelField("img_url", monster.img_url, _htmlContentStyle);

                EditorGUILayout.LabelField("DamageImmunities", monster.DamageImmunities, _htmlContentStyle);

                EditorGUILayout.LabelField("ConditionImmunities", monster.ConditionImmunities, _htmlContentStyle);

                EditorGUILayout.LabelField("DamageResistances", monster.DamageResistances, _htmlContentStyle);

                EditorGUILayout.LabelField("DamageVulnerabilities", monster.DamageVulnerabilities, _htmlContentStyle);
                HTMLField("Reactions", monster.Reactions);
            }
        }

        private void InitialiseCharacterData(Monster monster)
        {
            using var serializedObject = new SerializedObject(_currentCharacterData);
            using (var nameSerializedProperty = serializedObject.FindProperty("_name"))
            {
                nameSerializedProperty.stringValue = monster.name;
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
                DiceUtility.FromString(monster.HitPoints, out int faces, out int number, out int modifiers);

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
            using (var strSerializedProperty = serializedObject.FindProperty("_strength"))
            {
                strSerializedProperty.intValue = int.TryParse(monster.STR, out var value) ? value : 10;
            }
            using (var dexSerializedProperty = serializedObject.FindProperty("_dexterity"))
            {
                dexSerializedProperty.intValue = int.TryParse(monster.DEX, out var value) ? value : 10;
            }
            using (var conSerializedProperty = serializedObject.FindProperty("_constitution"))
            {
                conSerializedProperty.intValue = int.TryParse(monster.CON, out var value) ? value : 10;
            }
            using (var intSerializedProperty = serializedObject.FindProperty("_intelligence"))
            {
                intSerializedProperty.intValue = int.TryParse(monster.INT, out var value) ? value : 10;
            }
            using (var wisSerializedProperty = serializedObject.FindProperty("_wisdom"))
            {
                wisSerializedProperty.intValue = int.TryParse(monster.WIS, out var value) ? value : 10;
            }
            using (var chaSerializedProperty = serializedObject.FindProperty("_charisma"))
            {
                chaSerializedProperty.intValue = int.TryParse(monster.CHA, out var value) ? value : 10;
            }
            using (var savingThrowSerializedProperty = serializedObject.FindProperty("_savingThrows"))
            {
                SetSavingThrows(savingThrowSerializedProperty, monster.SavingThrows);
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
                profSerializedProperty.intValue = 2;
            }
            using (var actionsSerializedProperty = serializedObject.FindProperty("_actions"))
            {
                SetActions(actionsSerializedProperty, monster.Actions);
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

        private void SetSavingThrows(SerializedProperty savingThrowSerializedProperty, string savingThrows)
        {
            if (string.IsNullOrEmpty(savingThrows))
            {
                return;
            }
            var split = savingThrows.Split(',');
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
                curVal |= s switch
                {
                    "STR" => (int)StatType.Strength,
                    "DEX" => (int)StatType.Dexterity,
                    "CON" => (int)StatType.Constitution,
                    "INT" => (int)StatType.Intelligence,
                    "WIS" => (int)StatType.Wisdom,
                    "CHA" => (int)StatType.Charisma,
                    _ => 0
                };
            }
            savingThrowSerializedProperty.intValue = curVal;
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

        private void SetActions(SerializedProperty actionsSerializedProperty, string actions)
        {
            var rawEntries = GetEntries(actions);
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
            for (int i = 0; i < entries.Count; i++)
            {
                UnityEngine.Object data;
                if (TryMakeAttack(actionsSerializedProperty, i, entries[i].Item1, entries[i].Item2, out var attackData))
                {
                    data = attackData;
                }
                else if (TryMakeMultiattack(actionsSerializedProperty, i, entries[i].Item1, entries[i].Item2, out multiattackData))
                {
                    data = multiattackData;
                }
                else if (TryMakeAction(actionsSerializedProperty, i, entries[i].Item1, entries[i].Item2, out var actionData))
                {
                    data = actionData;
                }
                else
                {
                    continue;
                }
                data.name = entries[i].Item1;
                using (var elementProperty = actionsSerializedProperty.GetArrayElementAtIndex(i))
                {
                    elementProperty.objectReferenceValue = data;
                }
            }
            if (multiattackData != null)
            {

            }
        }

        private bool TryMakeMultiattack(SerializedProperty actionsSerializedProperty, int index, string title, string body, out MultiattackData multiattackData)
        {
            if (title.ToLower().Contains("multiattack"))
            {
                var inst = CreateInstance<MultiattackData>();
                using (var serializedObject = new SerializedObject(inst))
                {
                    using (var noteProperty = serializedObject.FindProperty("_note"))
                    {
                        noteProperty.stringValue = body;
                    }
                    serializedObject.ApplyModifiedProperties();
                }
                multiattackData = inst;
                return true;
            }
            multiattackData = null;
            return false;
        }

        private bool TryMakeAttack(SerializedProperty actionsSerializedProperty, int index, string title, string body, out AttackData attackData)
        {
            var split = body.Split(new[] { "<em>", "</em>" }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length < 4)
            {
                attackData = null;
                return false;
            }

            if (!GetAttackType(split[0], out var attackType))
            {
                attackData = null;
                return false;
            }
            if (!GetHitInfo(split[1], out var attackMod, out var range, out var maxRange, out var target))
            {
                attackData = null;
                return false;
            }
            if (!GetDamage(split[3], out var number, out var faces, out var modifier, out var damageType))
            {
                attackData = null;
                return false;
            }

            attackData = CreateInstance<AttackData>();

            using (var serializedObject = new SerializedObject(attackData))
            {
                using (var atkProperty = serializedObject.FindProperty("_attackType"))
                {
                    var names = atkProperty.enumNames;
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (names[i] == attackType.ToString())
                        {
                            atkProperty.enumValueIndex = i;
                            break;
                        }
                    }
                }
                using (var atkModProperty = serializedObject.FindProperty("_attackMod"))
                {
                    atkModProperty.intValue = attackMod;
                }
                using (var rangeProperty = serializedObject.FindProperty("_range"))
                {
                    rangeProperty.intValue = range;
                }
                using (var maxRangeProperty = serializedObject.FindProperty("_maxRange"))
                {
                    maxRangeProperty.intValue = maxRange;
                }
                using (var targetProperty = serializedObject.FindProperty("_target"))
                {
                    targetProperty.stringValue = target;
                }
                using (var damageDiceProperty = serializedObject.FindProperty("_damage"))
                {
                    using var numberProperty = damageDiceProperty.FindPropertyRelative("_number");
                    using var facesProperty = damageDiceProperty.FindPropertyRelative("_faces");
                    using var modifiersProperty = damageDiceProperty.FindPropertyRelative("_modifiers");

                    numberProperty.intValue = number;
                    facesProperty.intValue = faces;
                    modifiersProperty.intValue = modifier;
                }
                using (var damageProperty = serializedObject.FindProperty("_damageType"))
                {
                    var names = damageProperty.enumNames;
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (names[i] == damageType.ToString())
                        {
                            damageProperty.enumValueIndex = i;
                            break;
                        }
                    }
                }
                using (var noteProperty = serializedObject.FindProperty("_note"))
                {
                    noteProperty.stringValue = string.Join("\n", split.Skip(4).Select(s => s.Trim()).ToArray());
                }
                serializedObject.ApplyModifiedProperties();
            }
            return true;
        }

        private bool GetAttackType(string text, out AttackType type)
        {
            var attackTypeKey = text.Replace(" ", "").ToLower();
            if (attackTypeKey.EndsWith(":"))
            {
                attackTypeKey = attackTypeKey.Substring(0, attackTypeKey.Length - 1);
            }
            var attackTypes = Enum.GetValues(typeof(AttackType));
            foreach (var atk in attackTypes)
            {
                if (attackTypeKey == atk.ToString().ToLower())
                {
                    type = (AttackType)atk;
                    return true;
                }
            }
            type = default;
            return true;
        }

        private bool GetHitInfo(string text, out int attackMod, out int range, out int maxRange, out string target)
        {
            attackMod = 0;
            range = 0;
            maxRange = 0;
            target = string.Empty;
            var split = text.Split(',');

            if (split.Length < 3)
            {
                return false;
            }

            var attackModStr = split[0].Trim().ToLower();
            const string toHitSuffix = "to hit";
            if (!attackModStr.EndsWith(toHitSuffix))
            {
                return false;
            }
            attackModStr = attackModStr.Substring(0, attackModStr.Length - toHitSuffix.Length).Trim().TrimStart('+').Trim();
            if (!int.TryParse(attackModStr, out attackMod))
            {
                return false;
            }

            var reachStr = split[1].Trim();
            const string ftSuffix = "ft";
            var startIndex = GetIndex(reachStr, out var reachPrefix);
            var endIndex = reachStr.IndexOf(ftSuffix);

            if (startIndex == -1)
            {
                startIndex = 0;
            }
            if (endIndex == -1)
            {
                endIndex = reachStr.Length - 1;
            }
            reachStr = reachStr.Substring(startIndex + reachPrefix.Length, endIndex - startIndex - reachPrefix.Length).Trim();

            if (!TryGetRange(reachStr, out range, out maxRange))
            {
                var reachSplit = reachStr.Split(' ');
                var reachFound = false;
                for (int i = 0; i < reachSplit.Length; i++)
                {
                    if (TryGetRange(reachSplit[i], out range, out maxRange))
                    {
                        reachFound = true;
                        break;
                    }
                }
                if (!reachFound)
                {
                    return false;
                }
            }

            target = split[2].Trim().TrimEnd('.').Trim();
            return true;
        }

        private bool TryGetRange(string rangeString, out int range, out int maxRange)
        {
            if (int.TryParse(rangeString, out range))
            {
                maxRange = 0;
                return true;
            }
            else
            {
                var reachSplit = rangeString.Split('/');
                if (reachSplit.Length < 2)
                {
                    range = 0;
                    maxRange = 0;
                    return false;
                }
                if (!int.TryParse(reachSplit[0].Trim(), out range))
                {
                    range = 0;
                    maxRange = 0;
                    return false;
                }
                if (!int.TryParse(reachSplit[1].Trim(), out maxRange))
                {
                    range = 0;
                    maxRange = 0;
                    return false;
                }
                return true;
            }
        }

        private int GetIndex(string reachString, out string prefix)
        {
            var prefixes = new[] { "range", "reach" };
            for (int i = 0; i < prefixes.Length; i++)
            {
                var index = reachString.IndexOf(prefixes[i]);
                if (index != -1)
                {
                    prefix = prefixes[i];
                    return index;
                }
            }
            prefix = string.Empty;
            return 0;
        }

        private bool GetDamage(string text, out int number, out int faces, out int modifier, out DamageTypes damageType)
        {
            var bOpenInd = text.IndexOf('(');
            var bCloseInd = text.IndexOf(')');

            if (bOpenInd < 0 || bCloseInd < 0)
            {
                number = 0;
                faces = 0;
                modifier = 0;
                damageType = DamageTypes.none;
                return false;
            }
            var dice = text.Substring(bOpenInd + 1, bCloseInd - bOpenInd - 1);
            DiceUtility.FromString(dice, out number, out faces, out modifier);
            var remaining = text.Substring(bCloseInd + 1).Trim();

            var damageTypes = Enum.GetValues(typeof(DamageTypes));
            foreach (var dmg in damageTypes)
            {
                var dmgStr = dmg.ToString().ToLower();
                if (remaining.StartsWith(dmgStr))
                {
                    damageType = (DamageTypes)dmg;
                    return true;
                }
            }
            damageType = DamageTypes.none;
            return false;
        }

        private bool TryMakeSavingThrow(SerializedProperty actionsSerializedProperty, int index, string title, string body, out SavingThrowData savingThrowData)
        {
            if (TryMakeSavingThrow(body, out var savingThrow))
            {
                var inst = CreateInstance<SavingThrowData>();
                using (var serializedObject = new SerializedObject(inst))
                {
                    using (var dcProperty = serializedObject.FindProperty("_dc"))
                    {
                        dcProperty.intValue = savingThrow.DC ;
                    }
                    using (var savingThrowProperty = serializedObject.FindProperty("_savingThrow"))
                    {
                        savingThrowProperty.stringValue = body;
                    }
                    using (var failProperty = serializedObject.FindProperty("_failOutcome"))
                    {
                        failProperty.stringValue = body;
                    }
                    using (var successProperty = serializedObject.FindProperty("_successOutcome"))
                    {
                        successProperty.stringValue = body;
                    }
                }
                savingThrowData = inst;
                return true;
            }
            savingThrowData = null;
            return false;
        }

        private bool TryMakeSavingThrow(string body, out ISavingThrow savingThrow)
        {
            if (!body.ToLower().Contains("saving throw"))
            {

                savingThrow = null;
                return false;
            }
            savingThrow = default;
            return true;
        }

        private bool TryMakeAction(SerializedProperty actionsSerializedProperty, int index, string title, string body, out ActionData actionData)
        {
            var inst = CreateInstance<ActionData>();
            using (var serializedObject = new SerializedObject(inst))
            {
                using (var noteProperty = serializedObject.FindProperty("_note"))
                {
                    noteProperty.stringValue = body;
                }
                serializedObject.ApplyModifiedProperties();
            }
            actionData = inst;
            return true;
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
                    var body = GetEntries(html);
                    using (new EditorGUILayout.VerticalScope())
                    {
                        foreach (var s in body)
                        {
                            var title = s.Item1.Trim();
                            var explanation = s.Item2.Trim().Replace("<em>", "<b>").Replace("</em>", "</b>");
                            if (!string.IsNullOrEmpty(title))
                            {
                                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                            }

                            if (!string.IsNullOrEmpty(explanation))
                            {
                                EditorGUILayout.LabelField(explanation, _htmlContentStyle);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<(string, string)> GetEntries(string html)
        {
            var split = html.Split(new[] { "<p>", "</p>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in split)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    var b1 = ExtractFromTag("em", s, out var t1);
                    var b2 = ExtractFromTag("strong", t1, out var t2);
                    yield return (t2, $"{b2}{b1}");
                }
            }
        }

        private string ExtractFromTag(string tag, string body, out string withinTag)
        {
            var start = $"<{tag}>";
            var end = $"</{tag}>";

            var startIndex = body.IndexOf(start);
            var endIndex = body.IndexOf(end);

            if (startIndex < 0 || endIndex < 0)
            {
                withinTag = string.Empty;
                return body;
            }
            startIndex += start.Length;

            withinTag = body.Substring(startIndex, endIndex - startIndex).Trim();
            return body.Substring(endIndex + end.Length).Trim();
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

        private GUIStyle _indexReadout;
        private GUIStyle _statStyle;
        private GUIStyle _htmlContentStyle;
        private Monster[] _monsters = Array.Empty<Monster>();
        private int _currentMonsterIndex;
        private Vector2 _scroll;
        private CharacterData _currentCharacterData;
        private CharacterDataEditor _currentCharacterEditor;
        private const string JSON_PATH = "json_path";
    }
}