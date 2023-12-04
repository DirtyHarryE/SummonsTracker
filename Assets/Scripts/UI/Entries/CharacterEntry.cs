using SummonsTracker.Characters;
using SummonsTracker.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class CharacterEntry : MonoBehaviour
    {
        public Character Character { get; private set; }
        public MainPanel Panel { get; private set; }
        public string CharacterName
        {
            get => _name.text;
            set => _name.text = value;
        }
        public void Initialise(Character character, MainPanel panel)
        {
            Character = character;
            UpdateUI();
        }

        public void UpdateUI()
        {
            gameObject.name = Character.Name;
            _name.text = Character.Name;

            _condition.gameObject.SetActive(Character.Conditions != ConditionTypes.none);
            _condition.text = Character.Conditions.ToString();

            _tempHealth.gameObject.SetActive(Character.TemporaryHitPoints != 0);
            _tempHealth.text = $"+{Character.TemporaryHitPoints}";

            _curHealth.text = Character.Hitpoints.ToString();
            _maxHealth.text = Character.MaxHP.ToString();

            _descriptions.text = string.Join("\n", Character.Actions.Where(a => !(a is Multiattack)).Select(a => a.ToString()).ToArray());
        }

        public void ButtonClick()
        {
            GameManager.Instance.CharacterPanel.OpenCharacter(Character);
        }

        [SerializeField]
        private TextMeshProUGUI _name;
        [SerializeField]
        private TextMeshProUGUI _condition;
        [SerializeField]
        private TextMeshProUGUI _curHealth;
        [SerializeField]
        private TextMeshProUGUI _tempHealth;
        [SerializeField]
        private TextMeshProUGUI _maxHealth;
        [SerializeField]
        private TextMeshProUGUI _descriptions;

        private Character _character;
    }
}