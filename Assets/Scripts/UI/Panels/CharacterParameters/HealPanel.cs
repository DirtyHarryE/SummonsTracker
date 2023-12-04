using SummonsTracker.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SummonsTracker.UI
{
    public class HealPanel : CharacterParameterPanel
    {
        public override void Apply()
        {
            if (int.TryParse(_input.text, out var healAmount) && healAmount > 0)
            {
                if (_healButton.Selected)
                {
                    Character.Heal(healAmount);
                }
                else if (_tempHitpointsButton.Selected)
                {
                    Character.AddTemporaryHitPoints(healAmount);
                }
                if ((Character.Conditions & ConditionTypes.Unconscious) != 0)
                {
                    Character.RemoveCondition(ConditionTypes.Unconscious);
                }
            }
            base.Apply();
        }

        public override void Open(Character character)
        {
            base.Open(character);
            EventSystem.current.SetSelectedGameObject(_input.gameObject);
        }

        protected override void Awake()
        {
            base.Awake();
            _healButton.Initialise(HealButton);
            _tempHitpointsButton.Initialise(TempHPButton);

            _healButton.Select();
            _tempHitpointsButton.UnSelect();
        }

        private void HealButton(ToggleableButton button, bool selected)
        {
            _healButton.Select();
            _tempHitpointsButton.UnSelect();
        }

        private void TempHPButton(ToggleableButton button, bool selected)
        {
            _healButton.UnSelect();
            _tempHitpointsButton.Select();
        }

        [Space]
        [SerializeField]
        private TMP_InputField _input;
        [Space]
        [SerializeField]
        private ToggleableButton _healButton;
        [SerializeField]
        private ToggleableButton _tempHitpointsButton;
    }
}