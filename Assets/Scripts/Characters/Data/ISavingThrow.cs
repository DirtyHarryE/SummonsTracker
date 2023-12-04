namespace SummonsTracker.Characters
{
    public interface ISavingThrow
    {
        StatType SavingThrow { get; }
        int DC { get; }
        FailSaveOutome FailureSavingThrowOutcome { get; }
        SuccessSaveOutome SuccessSavingThrowOutcome { get; }
    }
}