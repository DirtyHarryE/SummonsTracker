using SummonsTracker.Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    [RequireComponent(typeof(Button))]
    public class AttackRollSelector : MonoBehaviour
    {
        public delegate void OnAttackRollSelectedDelegate(AttackRollSelector selector);

        public string AttackInstanceGUID { get; private set; }
        public int Result => _result;
        public bool IsCrossedOut => _checkMark.activeInHierarchy;

        public SaveTarget[] Targets { get; private set; }

        public void Initialise(string attackInstanceGUID, int result, bool isCrit, OnAttackRollSelectedDelegate onClick, SaveTarget[] targets)
        {
            AttackInstanceGUID = attackInstanceGUID;
            _result = result;
            _isCrit = isCrit;
            _onClick = onClick;
            _text.text = isCrit ? $"{result}!" : result.ToString();
            Targets = targets;
        }

        public void OnClick()
        {
            _onClick(this);
        }

        public void CrossOut(bool crossOut)
        {
            _checkMark.SetActive(crossOut);
        }

        private int _result;
        private bool _isCrit;
        public OnAttackRollSelectedDelegate _onClick;

        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private GameObject _checkMark;
    }
}