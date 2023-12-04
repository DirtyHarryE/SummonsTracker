using SummonsTracker.Characters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    [RequireComponent(typeof(Button))]
    public class SummonButton : MonoBehaviour
    {
        public CharacterData CharacterData => _characterData;
        public bool Selected => _selected;
        
        public void Initialise(CharacterData characterData, System.Action<SummonButton> onClick)
        {
            _characterData = characterData;
            _onClick = onClick;
            _text.text = characterData.Name;
        }

        public void OnClick()
        {
            _onClick?.Invoke(this);
        }

        public void Select()
        {
            _text.color = _selectedTextColor;
            _button.targetGraphic.color = _selectedButtonColor;
            _selected = true;
        }

        public void UnSelect()
        {
            _text.color = _unselectedTextColor;
            _button.targetGraphic.color = _unselectedButtonColor;
            _selected = false;
        }

        private void Reset()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _button = GetComponent<Button>();
        }

        private System.Action<SummonButton> _onClick;
        private bool _selected;
        private CharacterData _characterData;

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