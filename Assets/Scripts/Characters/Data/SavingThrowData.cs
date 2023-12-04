using UnityEngine;

namespace SummonsTracker.Characters
{
    [ActionMenuItem("Saving Throw")]
    public class SavingThrowData : ActionData, ISavingThrow
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
            return new SavingThrowAction(this);
        }
    }
}