using SummonsTracker.Characters;
using SummonsTracker.Text;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class SavingThrowReadout : Readout
    {
        public ISavingThrow SavingThrow => _savingThrow;

        public void Initialise(ISavingThrow savingThrow, string name, string note)
        {
            _savingThrow = savingThrow;

            TitleText.text = name;
            NoteText.text = note;
            _dcText.text = savingThrow.DC.ToString();
            _savingThrowText.text = TextUtils.DeCamelCase(savingThrow.SavingThrow.ToString());
        }

        public void SetDamageAndCondition(int damageAmount, DamageTypes damageType, ConditionTypes condition, string note)
        {
            if (damageAmount == 0 || damageType == DamageTypes.none)
            {
                _damageEntry.SetActive(false);
            }
            else
            {
                _damageEntry.SetActive(true);
                _damageAmount.text = damageAmount.ToString();
                _damageType.text = TextUtils.DeCamelCase(damageType.ToString());
            }
            if (condition == ConditionTypes.none)
            {
                _condition.gameObject.SetActive(false);
            }
            else
            {
                _condition.gameObject.SetActive(true);
                _condition.text = TextUtils.DeCamelCase(condition.ToString());
            }
            if (string.IsNullOrEmpty(note))
            {
                NoteText.gameObject.SetActive(false);
            }
            else
            {
                NoteText.gameObject.SetActive(true);
                NoteText.text = note;
            }
        }

        private ISavingThrow _savingThrow;

        [SerializeField]
        private TextMeshProUGUI _dcText;
        [SerializeField]
        private TextMeshProUGUI _savingThrowText;

        [Space]
        [SerializeField]
        private GameObject _damageEntry;
        [SerializeField]
        private TextMeshProUGUI _damageAmount;
        [SerializeField]
        private TextMeshProUGUI _damageType;

        [SerializeField]
        private TextMeshProUGUI _condition;
    }
}