using SummonsTracker.Characters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class CharacterPanel : Panel
    {
        public Character Character { get; private set; }

        public void OpenCharacter(Character character)
        {
            Character = character;
            _title.text = character.Name;
            Open();
        }

        public void RemoveCharacter()
        {

        }

        [SerializeField]
        private TextMeshProUGUI _title;
        
    }
}