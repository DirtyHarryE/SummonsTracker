using SummonsTracker.Characters;
using SummonsTracker.Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class RemoveCharacterPanel : CharacterParameterPanel
    {
        public override void Open(Character character)
        {
            base.Open(character);
            _body.text = $"Remove {character.Name}?";
        }

        public override void Apply()
        {
            GameManager.Instance.MainScene.RemoveCharacter(Character);
            base.Apply();
        }

        [SerializeField]
        private TextMeshProUGUI _body;
    }
}