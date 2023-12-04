using SummonsTracker.Characters;
using SummonsTracker.Manager;
using System;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class CharacterParameterPanel : Panel
    {
        public Character Character => GameManager.Instance.CharacterPanel.Character;

        public override void Open()
        {
            Open(Character);
        }
        public virtual void Open(Character character)
        {
            base.Open();
            _header.text = character.Name;
        }

        public virtual void Apply()
        {
            GameManager.Instance.MainScene.Open();
        }

        [SerializeField]
        private TextMeshProUGUI _header;
    }
}