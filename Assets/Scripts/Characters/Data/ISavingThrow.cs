namespace SummonsTracker.Characters
{
    public interface ISavingThrow
    {
        bool IsGrapple { get; }
        StatType SavingThrow { get; }
        int DC { get; }
        FailSaveOutcome FailureSavingThrowOutcome { get; }
        SuccessSaveOutcome SuccessSavingThrowOutcome { get; }
    }
}