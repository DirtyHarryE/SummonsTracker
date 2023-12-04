using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    [RequireComponent(typeof(Button))]
    public class ToggleableButton : MonoBehaviour
    {
        public delegate void OnToggleableClick(ToggleableButton button, bool selected);
        public bool Selected => _selected;

        public virtual void Initialise(OnToggleableClick onClick)
        {
            _onClick = onClick;
        }

        public virtual void OnClick()
        {
            _onClick?.Invoke(this, _selected);
        }

        public virtual void Select()
        {
            if (_text != null)
            {
                _text.color = _selectedTextColor;
            }
            _button.targetGraphic.color = _selectedButtonColor;
            _selected = true;
        }

        public virtual void UnSelect()
        {
            if (_text != null)
            {
                _text.color = _unselectedTextColor;
            }
            _button.targetGraphic.color = _unselectedButtonColor;
            _selected = false;
        }

        protected virtual void Reset()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _button = GetComponent<Button>();
        }

        private OnToggleableClick _onClick;
        private bool _selected;

        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Button _button;
        [Space]
        [SerializeField]
        private Color _unselectedTextColor = Color.black;
        [SerializeField]
        private Color _unselectedButtonColor = new Color(0.9803f, 0.9686f, 0.9176f, 1);
        [SerializeField]
        private Color _selectedTextColor = Color.white;
        [SerializeField]
        private Color _selectedButtonColor = new Color(0.6117f, 0.1686f, 0.1058f, 1f);
    }
}