using SummonsTracker.Characters;
using SummonsTracker.Save;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class SaveProfileEntry : MonoBehaviour
    {
        public Profile Profile => _profile;

        public void Initialise(LoadProfilePanel panel, Profile profile, int index)
        {
            _panel = panel;
            _index = index;
            _header.text = profile.Name;
            _summons.text = string.Join(", ", GetSummonsText(profile.SaveCharacters).ToArray());
            _profile = profile;
        }

        public void OnProfileButton()
        {
            _panel.LoadProfile(_index);
        }

        public void OnDeleteButton()
        {
            _panel.DeleteProfile(_index);
        }

        private IEnumerable<string> GetSummonsText(IEnumerable<SaveCharacter> saveCharacters)
        {
            if (saveCharacters.Any())
            {
                var dict = new Dictionary<string, int>();
                foreach (var saveCharacter in saveCharacters)
                {
                    var saveName = string.Empty;
                    var characterData = CharacterData.AllCharacters.FirstOrDefault(cData => cData.name == saveCharacter.DataName);
                    if (characterData != null)
                    {
                        if (saveCharacter.Name.StartsWith(characterData.Name))
                        {
                            saveName = characterData.Name;
                        }
                        else
                        {
                            saveName = saveCharacter.Name;
                        }
                    }
                    else
                    {
                        if (saveCharacter.Name.StartsWith(saveCharacter.DataName))
                        {
                            saveName = saveCharacter.DataName;
                        }
                        else
                        {
                            saveName = saveCharacter.Name;
                        }
                    }

                    if (!string.IsNullOrEmpty(saveName))
                    {
                        if (dict.ContainsKey(saveName))
                        {
                            dict[saveName] += 1;
                        }
                        else
                        {
                            dict.Add(saveName, 1);
                        }
                    }
                }
                foreach (var pair in dict)
                {
                    if (pair.Value <= 1)
                    {
                        yield return pair.Key;
                    }
                    else
                    {
                        yield return $"{pair.Key}\u00D7{pair.Value}";
                    }
                }
            }
            else
            {
                yield return "None";
            }
        }

        private LoadProfilePanel _panel;
        private Profile _profile;
        private int _index;

        [SerializeField]
        private TextMeshProUGUI _header;
        [SerializeField]
        private TextMeshProUGUI _summons;
    }
}