using SummonsTracker.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Save
{
    [Serializable]
    public class SaveSavingThrow
    {
        public SaveSavingThrow(bool isGrapple, StatType savingThrow, int dC, SaveThrowOutcome failureSavingThrowOutcome, SaveThrowOutcome successSavingThrowOutcome)
        {
            IsGrapple = isGrapple;
            SavingThrow = savingThrow;
            DC = dC;
            FailureSavingThrowOutcome = failureSavingThrowOutcome;
            SuccessSavingThrowOutcome = successSavingThrowOutcome;
        }

        public bool IsGrapple { get; }
        public StatType SavingThrow { get; }
        public int DC { get; }
        public SaveThrowOutcome FailureSavingThrowOutcome { get; }
        public SaveThrowOutcome SuccessSavingThrowOutcome { get; }

        public override string ToString()
        {
            return $"DC {DC} {(IsGrapple ? "Grapple" : $"{SavingThrow} saving throw")}, Fail = ({FailureSavingThrowOutcome}), Success = ({SuccessSavingThrowOutcome})";
        }
    }
}