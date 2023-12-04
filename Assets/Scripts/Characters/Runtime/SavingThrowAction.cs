namespace SummonsTracker.Characters
{
    public class SavingThrowAction : Action, ISavingThrow
    {
        public SavingThrow SavingThrow;
        public SavingThrowAction(SavingThrowData savingThrowData) : base(savingThrowData)
        {
            SavingThrow = new SavingThrow(savingThrowData.name, savingThrowData.SavingThrow, savingThrowData.DC, savingThrowData.FailureSavingThrowOutcome, savingThrowData.SuccessSavingThrowOutcome);
        }

        public override string ToString()
        {
            return $"<b>{Name}</b>. {SavingThrow}. {Note}";
        }

        public StatType SavingThrowType => SavingThrow.SavingThrowType;

        public int DC => SavingThrow.DC;

        public FailSaveOutome FailureSavingThrowOutcome => SavingThrow.FailSavingThrowOutcome;

        public SuccessSaveOutome SuccessSavingThrowOutcome => SavingThrow.SuccessSavingThrowOutcome;

        StatType ISavingThrow.SavingThrow => SavingThrow.SavingThrowType;
    }
}