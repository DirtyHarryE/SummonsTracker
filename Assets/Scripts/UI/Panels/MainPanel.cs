using SummonsTracker.Characters;
using SummonsTracker.Manager;
using SummonsTracker.Save;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummonsTracker.UI
{
    public class MainPanel : Panel
    {
        public IReadOnlyList<Character> ConcentrationCharacters => _concentrationCharacters;
        public IReadOnlyList<Character> Characters => _characters;

        public void SummonCharacters(IEnumerable<Character> characters, bool concentrate = false)
        {
            if (concentrate)
            {
                RemoveCharacters(_concentrationCharacters);
            }
            _concentrationCharacters.Clear();
            var l = concentrate ? _concentrationCharacters : _characters;
            var charas = characters.ToArray();
            foreach (var group in l.Concat(charas).GroupBy(c => c.CharacterData))
            {
                var dataName = group.Key.Name.Trim();
                var max = 0;
                foreach (var c in group)
                {
                    if (GetNameAndNumber(c.Name, dataName, out int num))
                    {
                        if (max < num)
                        {
                            max = num;
                        }
                    }
                }
                max += 1;
                var i = 0;
                var firstNamed = false;
                foreach (var c in group)
                {
                    if (c.Name.Trim() == dataName)
                    {
                        if (!firstNamed)
                        {
                            firstNamed = true;
                        }
                        else
                        {
                            var num = max + i++;
                            c.Name = $"{c.Name.Trim()} {num}";
                        }
                    }
                    var existingEntry = _entries.FirstOrDefault(e => e.Character == c);
                    if (existingEntry != null)
                    {
                        existingEntry.CharacterName = c.Name;
                    }
                }
            }
            var wizardLevel = 0;
            var proficiency = 0;
            if (GameManager.Instance != null && GameManager.Instance.StatScene)
            {
                wizardLevel = GameManager.Instance.StatScene.WizardLevel;
                proficiency = GameManager.Instance.StatScene.Proficiency;
            }
            for (int i = 0; i < charas.Length; i++)
            {
                if (charas[i].Creature == CreatureType.Undead)
                {
                    charas[i].Hitpoints += wizardLevel;
                    charas[i].MaxHP += wizardLevel;
                    for (int j = 0; j < charas[i].Actions.Length; j++)
                    {
                        if (charas[i].Actions[j] is Attack atk && atk.Damages.Length > 0)
                        {
                            atk.Damages[0].DamageDice = new Rolling.Dice(atk.Damages[0].DamageDice.Number, atk.Damages[0].DamageDice.Faces, atk.Damages[0].DamageDice.Modifiers + proficiency);
                        }
                    }
                }
            }
            l.AddRange(charas);
            foreach (var character in charas)
            {
                _entries.Add(InitEntry(character, concentrate));
            }
            UpdateUI();
        }

        public void RemoveCharacter(Character toDelete)
        {
            RemoveCharacters(_entries.Where(c => c.Character == toDelete));
        }
        public void RemoveCharacters(IEnumerable<Character> toDelete)
        {
            RemoveCharacters(_entries.Where(c => toDelete.Contains(c.Character)));
        }
        public void RemoveCharacters(IEnumerable<CharacterEntry> toDelete)
        {
            var d = toDelete.ToArray();
            for (int i = d.Length - 1; i >= 0; i--)
            {
                var character = d[i].Character;
                if (_concentrationCharacters.Contains(character))
                {
                    _concentrationCharacters.Remove(character);
                }
                if (_characters.Contains(character))
                {
                    _characters.Remove(character);
                }
                _entries.Remove(d[i]);
                UnityEngine.Object.Destroy(d[i].gameObject);
            }
            UpdateUI();
        }

        public override void Open()
        {
            ClearEntries();
            UpdateUI();
            base.Open();
        }

        public override void Close()
        {
            SaveManager.Instance.CurrentProfile.SaveCharacters = GetSaveCharacters().ToArray();
            SaveManager.Instance.Save();
            base.Close();
        }

        public void Attack()
        {
            GameManager.Instance.AttackPanel.Initialise(GetCharacters());
        }

        private IEnumerable<Character> GetCharacters()
        {
            foreach (var character in _concentrationCharacters.Concat(_characters))
            {
                if ((character.Conditions & ConditionTypes.Unconscious) == 0)
                {
                    yield return character;
                }
            }
        }

        private bool GetNameAndNumber(string characterName, string dataName, out int num)
        {
            var c = characterName.Trim();
            var d = dataName.Trim();
            if (c.StartsWith(d))
            {
                var sub = c.Substring(d.Length).Trim();
                if (int.TryParse(sub, out int suffix))
                {
                    num = suffix;
                    return true;
                }
            }
            num = 0;
            return false;
        }

        private CharacterEntry InitEntry(Character character, bool concentration)
        {
            GameObject instGO = GameObject.Instantiate(_characterEntryPrefab, _content);
            var charEntry = instGO.GetComponent<CharacterEntry>();
            charEntry.Initialise(character, this);

            return charEntry;
        }

        private void ClearEntries()
        {
            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(_entries[i].gameObject);
            }
            _concentrationCharacters.Clear();
            _characters.Clear();
            _entries.Clear();
        }

        private void LoadEntries()
        {
            for (int i = 0; i < SaveManager.Instance.CurrentProfile.SaveCharacters.Length; i++)
            {
                var c = SaveManager.Instance.CurrentProfile.SaveCharacters[i];
                if (c.Concentration)
                {
                    var character = new Character()
                }
            }
        }

        private void UpdateUI()
        {
            var concentrationIndex = 0;
            var normalIndex = 0;
            var unconciousIndex = 0;
            if (_entries.Any())
            {
                _scrollView.gameObject.SetActive(true);
                _getStartedText.gameObject.SetActive(false);
                foreach (var entry in _entries)
                {
                    entry.UpdateUI();
                    if ((entry.Character.Conditions & ConditionTypes.Unconscious) != 0)
                    {
                        entry.transform.SetSiblingIndex(_unconciousHeader.transform.GetSiblingIndex() + 1 + unconciousIndex++);
                    }
                    else if (_concentrationCharacters.Contains(entry.Character))
                    {
                        entry.transform.SetSiblingIndex(_concentrationHeader.transform.GetSiblingIndex() + 1 + concentrationIndex++);
                    }
                    else
                    {
                        entry.transform.SetSiblingIndex(_unConcentrationHeader.transform.GetSiblingIndex() + 1 + normalIndex++);
                    }
                }
            }
            else
            {
                _scrollView.gameObject.SetActive(false);
                _getStartedText.gameObject.SetActive(true);
            }
            _concentrationHeader.gameObject.SetActive(concentrationIndex > 0);
            _unConcentrationHeader.gameObject.SetActive(normalIndex > 0);
            _unconciousHeader.gameObject.SetActive(unconciousIndex > 0);
        }

        private IEnumerable<SaveCharacter> GetSaveCharacters()
        {
            foreach (var character in _concentrationCharacters)
            {
                yield return GetSaveCharacter(character, true);
            }
            foreach (var character in _characters)
            {
                yield return GetSaveCharacter(character, false);
            }
        }

        private SaveCharacter GetSaveCharacter(Character character, bool concentration)
        {
            return new SaveCharacter(dataName: character.CharacterData.name,
                                     name: character.Name,
                                     hP: character.Hitpoints,
                                     maxHP: character.MaxHP,
                                     tempHP: character.TemporaryHitPoints,
                                     condition: character.Conditions,
                                     concentration: concentration);
        }

        [SerializeField]
        private GameObject _scrollView;
        [SerializeField]
        private GameObject _getStartedText;
        [Space]
        [SerializeField]
        private GameObject _characterEntryPrefab;
        [Space]
        [SerializeField]
        private Transform _content;
        [SerializeField, FormerlySerializedAs("_concentrationheader")]
        private GameObject _concentrationHeader;
        [SerializeField, FormerlySerializedAs("_unConcentrationheader")]
        private GameObject _unConcentrationHeader;
        [SerializeField]
        private GameObject _unconciousHeader;

        private List<Character> _concentrationCharacters = new List<Character>();
        private List<Character> _characters = new List<Character>();
        private List<CharacterEntry> _entries = new List<CharacterEntry>();
    }
}