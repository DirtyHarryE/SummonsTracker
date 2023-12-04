using SummonsTracker.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    [RequireComponent(typeof(Button))]
    public class SavingThrowSelector : MonoBehaviour
    {
        public delegate void OnSavingThrowSelectedDelegate(SavingThrowSelector selector);

        public string AttackInstanceGUID { get; private set; }
        public int DC => _dc;
        public bool IsCrossedOut => _isCrossOut;
        public bool IsLocked => _isLocked;
        public Attack TiedAttack;

        public ISavingThrow SavingThrow { get; private set; }

        public void Initialise(string guid, ISavingThrow savingThrow, OnSavingThrowSelectedDelegate onClick)
        {
            AttackInstanceGUID = guid;
            SavingThrow = savingThrow;
            _dc = savingThrow.DC;
            _onClick = onClick;
            _text.text = savingThrow.DC.ToString();
        }

        public void OnClick()
        {
            _onClick(this);
        }

        public void CrossOut(bool crossOut)
        {
            _isCrossOut = crossOut;
            UpdateIcons();
        }

        public void Lock(bool locked)
        {
            _isLocked = locked;
            UpdateIcons();
        }

        private void UpdateIcons()
        {
            if (IsLocked)
            {
                _text.color = _lockedTextColor;
                _checkMark.SetActive(false);
                _lockIcon.SetActive(true);
            }
            else if (IsCrossedOut)
            {
                _text.color = _crossedOutTextColor;
                _checkMark.SetActive(true);
                _lockIcon.SetActive(false);
            }
            else
            {
                _text.color = _normalTextColor;
                _checkMark.SetActive(false);
                _lockIcon.SetActive(false);
            }
        }

        private int _dc;
        private bool _isCrossOut;
        private bool _isLocked;
        private OnSavingThrowSelectedDelegate _onClick;

        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private GameObject _checkMark;
        [SerializeField]
        private GameObject _lockIcon;
        [Header("Colours")]
        [SerializeField]
        private Color _normalTextColor = Color.black;
        [SerializeField]
        private Color _crossedOutTextColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        [SerializeField]
        private Color _lockedTextColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        [SerializeField]
        private Color _crossColor = new Color(0.6117f, 0.1686f, 0.1058f, 0.8f);
    }
}