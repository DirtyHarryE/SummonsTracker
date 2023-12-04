using UnityEngine;

namespace SummonsTracker.Characters
{
    [ActionMenuItem("Attack/Single Damage/Saving Throw")]
    public class AttackAndSavingThrowData : AttackData, ISavingThrow
    {
        public StatType SavingThrow { get => _savingThrow; }
        public int DC => _dc;
        public FailSaveOutome FailureSavingThrowOutcome => _failOutcome;
        public SuccessSaveOutome SuccessSavingThrowOutcome => _successOutcome;

        [SerializeField]
        private int _dc;
        [SerializeField]
        private StatType _savingThrow;
        [SerializeField]
        private FailSaveOutome _failOutcome = FailSaveOutome.None;
        [SerializeField]
        private SuccessSaveOutome _successOutcome = SuccessSaveOutome.None;

        public override Action Instantiate()
        {
            return new Attack(this) { SavingThrow = new SavingThrow(name, _savingThrow, _dc, _failOutcome, _successOutcome) };
        }
    }
}