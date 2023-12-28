using SummonsTracker.Rolling;
using System.Text;

namespace SummonsTracker.Characters
{
    public class SavingThrow : ISavingThrow
    {
        public string Name;
        public StatType SavingThrowType;
        public int DC;
        public bool IsGrapple;
        public FailSaveOutcome FailSavingThrowOutcome = FailSaveOutcome.None;
        public SuccessSaveOutcome SuccessSavingThrowOutcome = SuccessSaveOutcome.None;

        public SavingThrow(string name, StatType savingThrowType, int dC, FailSaveOutcome failSavingThrowOutcome, SuccessSaveOutcome successSavingThrowOutcome)
        {
            Name = name;
            SavingThrowType = savingThrowType;
            DC = dC;
            FailSavingThrowOutcome = failSavingThrowOutcome;
            SuccessSavingThrowOutcome = successSavingThrowOutcome;
        }

        public SavingThrow(string name, bool isGrapple, int dC, FailSaveOutcome failSavingThrowOutcome, SuccessSaveOutcome successSavingThrowOutcome)
        {
            Name = name;
            IsGrapple = isGrapple;
            DC = dC;
            FailSavingThrowOutcome = failSavingThrowOutcome;
            SuccessSavingThrowOutcome = successSavingThrowOutcome;
        }

        public SavingThrow(ISavingThrow savingThrow, string name)
        {
            Name = name;
            IsGrapple = savingThrow.IsGrapple;
            DC = savingThrow.DC;
            FailSavingThrowOutcome = savingThrow.FailureSavingThrowOutcome;
            SuccessSavingThrowOutcome = savingThrow.SuccessSavingThrowOutcome;
        }

        StatType ISavingThrow.SavingThrow => SavingThrowType;
        int ISavingThrow.DC => DC;
        FailSaveOutcome ISavingThrow.FailureSavingThrowOutcome => FailSavingThrowOutcome;
        SuccessSaveOutcome ISavingThrow.SuccessSavingThrowOutcome => SuccessSavingThrowOutcome;
        bool ISavingThrow.IsGrapple => IsGrapple;

        public override string ToString() => SavingThrowHelper.SavingThrowToString(this);

        public static SavingThrow Copy(SavingThrow savingThrow, int newDC)
        {
            return savingThrow.IsGrapple
                ? new SavingThrow(savingThrow.Name, false, newDC, savingThrow.FailSavingThrowOutcome, savingThrow.SuccessSavingThrowOutcome)
                : new SavingThrow(savingThrow.Name, savingThrow.SavingThrowType, newDC, savingThrow.FailSavingThrowOutcome, savingThrow.SuccessSavingThrowOutcome);
        }

    }
}