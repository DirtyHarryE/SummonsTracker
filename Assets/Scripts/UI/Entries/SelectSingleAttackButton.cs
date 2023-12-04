using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    [RequireComponent(typeof(Button))]
    public class SelectSingleAttackButton : SelectAttackButton
    {
        public delegate void AttackButtonClick(SelectSingleAttackButton button);

        public Button Button
        {
            get
            {
                if (_button == null)
                {
                    _button = GetComponent<Button>();
                }
                return _button;
            }
        }

        public bool Selected { get; private set; }

        public void Initialise(AttackButtonClick onClick, string text, int attackIndex, int buttonIndex)
        {
            _onClick = onClick;
            Text.text = text;
            AttackIndex = attackIndex;
            ButtonIndex = buttonIndex;
        }

        public void Select()
        {
            Text.color = SelectedTextColor;
            Button.targetGraphic.color = SelectedButtonColor;
            Selected = true;
        }

        public void UnSelect()
        {
            Text.color = UnselectedTextColor;
            Button.targetGraphic.color = UnselectedButtonColor;
            Selected = false;
        }

        private void OnClick()
        {
            _onClick?.Invoke(this);
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }
        private Button _button;
        protected AttackButtonClick _onClick;
    }
}