using SummonsTracker.Multiattacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public class SelectMultiAttackButton : SelectAttackButton, IMultiattack
    {
        public delegate bool OnValidateDelegate(SelectMultiAttackButton button, int number);
        public delegate void OnMultiValueSetDelegate(SelectMultiAttackButton button, int number);

        public int AttackNumber
        {
            get => _number;
            set
            {
                var changed = false;
                if (_onValidate(this, value))
                {
                    changed = _number != value;
                    _number = value;
                    _numberText.text = value.ToString();

                    _decreaseButton.interactable = AttackNumber > 0;
                }
                _increaseButton.interactable = _onValidate(this, value + 1);
                if (changed)
                {
                    _onValueSet(this, value);
                }
            }
        }

        public void SetNumberDirect(int number)
        {
            _number = number;
            _numberText.text = number.ToString();
        }

        public void Validate()
        {
            _decreaseButton.interactable = AttackNumber > 0;
            _increaseButton.interactable = _onValidate(this, AttackNumber + 1);
        }

        public void Initialise(OnValidateDelegate onValidate, OnMultiValueSetDelegate onValueSet, string text, int attackIndex, int buttonIndex)
        {
            _onValidate = onValidate;
            _onValueSet = onValueSet;
            Text.text = text;
            AttackIndex = attackIndex;
            ButtonIndex = buttonIndex;
        }

        public void Increment(int increment = 1)
        {
            AttackNumber = Mathf.Max(AttackNumber + increment, 0);
        }
        private OnValidateDelegate _onValidate;
        private OnMultiValueSetDelegate _onValueSet;

        private int _number;
        [SerializeField]
        private TextMeshProUGUI _numberText;
        [SerializeField]
        private Button _increaseButton;
        [SerializeField]
        private Button _decreaseButton;
    }
}