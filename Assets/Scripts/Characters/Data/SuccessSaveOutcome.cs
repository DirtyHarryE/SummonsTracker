using SummonsTracker.Rolling;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummonsTracker.Characters
{
    public enum SuccessSavingThrowOutcomes
    {
        Nothing,
        Half,
        Damage,
        Condition,
        DamageAndCondition,
        Other
    }
    [System.Serializable]
    public struct SuccessSaveOutcome
    {
        public SuccessSavingThrowOutcomes SuccessSaveType => _successSaveType;
        public Dice Damage => _successSaveType switch
        {
            SuccessSavingThrowOutcomes.Damage => _damage,
            SuccessSavingThrowOutcomes.DamageAndCondition => _damage,
            _ => Dice.None,
        };
        public DamageTypes DamageType => _successSaveType switch
        {
            SuccessSavingThrowOutcomes.Damage => _damageType,
            SuccessSavingThrowOutcomes.DamageAndCondition => _damageType,
            _ => DamageTypes.none,
        };
        public ConditionTypes Condition => _successSaveType switch
        {
            SuccessSavingThrowOutcomes.Condition => _condition,
            SuccessSavingThrowOutcomes.DamageAndCondition => _condition,
            _ => ConditionTypes.none,
        };
        public string OutcomeNote => _successSaveType switch
        {
            SuccessSavingThrowOutcomes.Nothing => string.Empty,
            _ => _outcomeNote,
        };

        public bool IsDamage => _successSaveType switch
        {
            SuccessSavingThrowOutcomes.Damage => true,
            SuccessSavingThrowOutcomes.DamageAndCondition => true,
            _ => false,
        };
        public bool IsCondition => _successSaveType switch
        {
            SuccessSavingThrowOutcomes.Condition => true,
            SuccessSavingThrowOutcomes.DamageAndCondition => true,
            _ => false,
        };

        public SuccessSaveOutcome(SuccessSavingThrowOutcomes SuccessSaveType, Dice damage, DamageTypes damageType, ConditionTypes condition, string outcomeNote)
        {
            _successSaveType = SuccessSaveType;
            _damage = damage;
            _damageType = damageType;
            _condition = condition;
            _outcomeNote = outcomeNote;
        }

        public static SuccessSaveOutcome None => new SuccessSaveOutcome(SuccessSavingThrowOutcomes.Nothing, Dice.None, DamageTypes.none, ConditionTypes.none, string.Empty);

        [SerializeField, FormerlySerializedAs("_SuccessSaveType")]
        private SuccessSavingThrowOutcomes _successSaveType;
        [SerializeField]
        private Dice _damage;
        [SerializeField]
        private DamageTypes _damageType;
        [SerializeField]
        private ConditionTypes _condition;
        [SerializeField, TextArea]
        private string _outcomeNote;
    }
}