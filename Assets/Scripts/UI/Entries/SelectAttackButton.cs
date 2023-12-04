using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public abstract class SelectAttackButton : MonoBehaviour
    {
        public TextMeshProUGUI Text => _text;
        public int AttackIndex { get; protected set; }
        public int ButtonIndex { get; protected set; }

        protected Color UnselectedTextColor => _unselectedTextColor;
        protected Color UnselectedButtonColor => _unselectedButtonColor;
        protected Color SelectedTextColor => _selectedTextColor;
        protected Color SelectedButtonColor => _selectedButtonColor;


        [SerializeField]
        private TextMeshProUGUI _text;
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