namespace SummonsTracker.Characters
{
    public class SavingThrowAction : Action, ISavingThrow
    {
        public SavingThrow SavingThrow;
        public SavingThrowAction(SavingThrowData savingThrowData) : base(savingThrowData)
        {
            SavingThrow = savingThrowData.IsGrapple 
                ? new SavingThrow(savingThrowData.name, true, savingThrowData.DC, savingThrowData.FailureSavingThrowOutcome, savingThrowData.SuccessSavingThrowOutcome)
                : new SavingThrow(savingThrowData.name, savingThrowData.SavingThrow, savingThrowData.DC, savingThrowData.FailureSavingThrowOutcome, savingThrowData.SuccessSavingThrowOutcome);
        }

        public SavingThrowAction(string name, string note, SavingThrow savingThrow) : base(name, note)
        {
            SavingThrow = savingThrow.IsGrapple
                ? new SavingThrow(name, true, savingThrow.DC, savingThrow.FailSavingThrowOutcome, savingThrow.SuccessSavingThrowOutcome)
                : new SavingThrow(name, savingThrow.SavingThrowType, savingThrow.DC, savingThrow.FailSavingThrowOutcome, savingThrow.SuccessSavingThrowOutcome);
        }

        public override string ToString()
        {
            return $"<b>{Name}</b>. {SavingThrow}. {Note}";
        }

        public StatType SavingThrowType => SavingThrow.SavingThrowType;

        public int DC => SavingThrow.DC;

        public FailSaveOutcome FailureSavingThrowOutcome => SavingThrow.FailSavingThrowOutcome;

        public SuccessSaveOutcome SuccessSavingThrowOutcome => SavingThrow.SuccessSavingThrowOutcome;

        StatType ISavingThrow.SavingThrow => SavingThrow.SavingThrowType;

        public bool IsGrapple => SavingThrow.IsGrapple;
    }
}