using SummonsTracker.Characters;
using SummonsTracker.Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SummonsTracker.UI
{
    public class RenameCharacterPanel : CharacterParameterPanel
    {
        public override void Open(Character character)
        {
            base.Open(character);
            _body.text = $"Rename {character.Name}";
            _input.text = character.Name;
            EventSystem.current.SetSelectedGameObject(_input.gameObject);
        }

        public override void Apply()
        {
            Character.Name = _input.text;
            base.Apply();
        }

        [SerializeField]
        private TextMeshProUGUI _body;
        [SerializeField]
        private TMP_InputField _input;
    }
}