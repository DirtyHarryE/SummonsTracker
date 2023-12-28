using SummonsTracker.Characters;
using SummonsTracker.Save;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class ConditionReadout : MonoBehaviour
    {
        public SaveTarget Target;

        public ConditionTypes Condition
        {
            get => _condition;
            set
            {
                _condition = value;
                _conditionText.text = value.ToString();
            }
        }
        public string OutcomeNote
        {
            get => _outcomeNote;
            set
            {
                _outcomeNote = value;
                _outcomeText.text = value;
            }
        }

        private ConditionTypes _condition;
        private string _outcomeNote;

        [SerializeField]
        private TextMeshProUGUI _conditionText;
        [SerializeField]
        private TextMeshProUGUI _outcomeText;
    }
}