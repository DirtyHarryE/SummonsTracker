using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public class NumberInput : Panel
    {
        public void InputButton(int index)
        {
            _text.text = _text.text == "0" ? index.ToString() : $"{_text.text}{index}";
        }

        public void BackspaceButton()
        {
            if (!string.IsNullOrEmpty(_text.text))
            {
                _text.text = _text.text.Substring(0, _text.text.Length - 1);
            }
        }

        public void AcceptButton()
        {

        }

        public void CloseButton()
        {

        }
        [Header("Number Input")]
        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private Button[] _buttons;
        [SerializeField]
        private Button[] _backspace;
        [SerializeField]
        private Button[] _accept;
        [SerializeField]
        private Button[] _close;
    }
}