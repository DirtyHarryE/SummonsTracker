using UnityEngine;

namespace SummonsTracker.Characters
{
    [ActionMenuItem("Attack/Single Damage/Saving Throw")]
    public class AttackAndSavingThrowData : AttackData, ISavingThrow
    {
        public StatType SavingThrow { get => _savingThrow; }
        public int DC => _dc;
        public bool IsGrapple => _isGrapple;
        public FailSaveOutcome FailureSavingThrowOutcome => _failOutcome;
        public SuccessSaveOutcome SuccessSavingThrowOutcome => _successOutcome;

        [SerializeField]
        private int _dc;
        [SerializeField]
        private bool _isGrapple;
        [SerializeField]
        private StatType _savingThrow;
        [SerializeField]
        private FailSaveOutcome _failOutcome = FailSaveOutcome.None;
        [SerializeField]
        private SuccessSaveOutcome _successOutcome = SuccessSaveOutcome.None;

        public override Action Instantiate()
        {
            return new Attack(this) { SavingThrow = _isGrapple 
                ? new SavingThrow(name, true, _dc, _failOutcome, _successOutcome)
                : new SavingThrow(name, _savingThrow, _dc, _failOutcome, _successOutcome)
            };
        }
    }
}