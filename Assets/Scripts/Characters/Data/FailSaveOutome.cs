using SummonsTracker.Rolling;
using UnityEngine;
namespace SummonsTracker.Characters
{
    public enum FailSavingThrowOutcomes
    {
        Nothing,
        Damage,
        Condition,
        DamageAndCondition,
        Other
    }
    [System.Serializable]
    public struct FailSaveOutome
    {
        public FailSavingThrowOutcomes FailSaveType => _failSaveType;
        public Dice Damage => _failSaveType switch
        {
            FailSavingThrowOutcomes.Damage => _damage,
            FailSavingThrowOutcomes.DamageAndCondition => _damage,
            _ => Dice.None,
        };
        public DamageTypes DamageType => _failSaveType switch
        {
            FailSavingThrowOutcomes.Damage => _damageType,
            FailSavingThrowOutcomes.DamageAndCondition => _damageType,
            _ => DamageTypes.none,
        };
        public ConditionTypes Condition => _failSaveType switch
        {
            FailSavingThrowOutcomes.Condition => _condition,
            FailSavingThrowOutcomes.DamageAndCondition => _condition,
            _ => ConditionTypes.none,
        };
        public string OutcomeNote => _failSaveType switch
        {
            FailSavingThrowOutcomes.Nothing => string.Empty,
            _ => _outcomeNote,
        };

        public bool IsDamage => _failSaveType switch
        {
            FailSavingThrowOutcomes.Damage => true,
            FailSavingThrowOutcomes.DamageAndCondition => true,
            _ => false,
        };
        public bool IsCondition => _failSaveType switch
        {
            FailSavingThrowOutcomes.Condition => true,
            FailSavingThrowOutcomes.DamageAndCondition => true,
            _ => false,
        };

        public FailSaveOutome(FailSavingThrowOutcomes failSaveType, Dice damage, DamageTypes damageType, ConditionTypes condition, string outcomeNote)
        {
            _failSaveType = failSaveType;
            _damage = damage;
            _damageType = damageType;
            _condition = condition;
            _outcomeNote = outcomeNote;
        }

        public static FailSaveOutome None => new FailSaveOutome(FailSavingThrowOutcomes.Nothing, Dice.None, DamageTypes.none, ConditionTypes.none, string.Empty);

        [SerializeField]
        private FailSavingThrowOutcomes _failSaveType;
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