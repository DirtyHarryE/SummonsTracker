using SummonsTracker.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public class SaveTargetEntry : MonoBehaviour
    {
        public SaveTarget Target { get; private set; }

        public bool TargetEnabled
        {
            get => _targetEnabled;
            set
            {
                _targetEnabled = value;
                UpdateUI();
            }
        }

        public void ToggleActive()
        {
            TargetEnabled = !TargetEnabled;
        }

        public void Initialise(SaveTarget target)
        {
            Target = target;
            _text.text = target.TargetName;
            TargetEnabled = true;
        }

        #region Unity Messages

        private void Reset()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _line = GetComponentInChildren<Image>();
        }

        #endregion

        #region Private

        private void UpdateUI()
        {
            _text.color = _targetEnabled ? _textActiveColor : _textInactiveColor;
            _line.color = _targetEnabled ? _lineActiveColor : _lineInactiveColor;
        }

        private bool _targetEnabled;

        [Header("Components")]
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Image _line;

        [Header("Colours")]
        [SerializeField]
        private Color _textActiveColor = Color.black;
        [SerializeField]
        private Color _lineActiveColor = new Color(0.6117647f, 0.1686275f, 0.1058824f);
        [SerializeField]
        private Color _textInactiveColor = Color.gray;
        [SerializeField]
        private Color _lineInactiveColor = Color.gray;

        #endregion
    }
}