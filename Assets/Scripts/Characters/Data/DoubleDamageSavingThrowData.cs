using UnityEngine;

namespace SummonsTracker.Characters
{
    [ActionMenuItem("Attack/Double Damage/Saving Throw")]
    public class DoubleDamageSavingThrowData : AttackSecondDamageData, ISavingThrow
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
            var atk = (Attack) base.Instantiate();
            atk.SavingThrow = IsGrapple
                ? new SavingThrow(name, true, _dc, _failOutcome, _successOutcome)
                : new SavingThrow(name, _savingThrow, _dc, _failOutcome, _successOutcome);
            return atk;
        }
    }
}