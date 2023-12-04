using SummonsTracker.Characters;
using SummonsTracker.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class DamageRollReadout : MonoBehaviour
    {
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                _numberText.text = value.ToString();
            }
        }
        public DamageTypes DamageType
        {
            get => damageType;
            set
            {
                damageType = value;
                _damageTypeText.text = TextUtils.DeCamelCase(damageType.ToString());
            }
        }

        private int _number;
        private DamageTypes damageType;

        [SerializeField]
        private TextMeshProUGUI _numberText;
        [SerializeField]
        private TextMeshProUGUI _damageTypeText;
    }
}