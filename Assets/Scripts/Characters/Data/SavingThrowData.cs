using UnityEngine;

namespace SummonsTracker.Characters
{
    [ActionMenuItem("Saving Throw")]
    public class SavingThrowData : ActionData, ISavingThrow
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
            return new SavingThrowAction(this);
        }
    }
}