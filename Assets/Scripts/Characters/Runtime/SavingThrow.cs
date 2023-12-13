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

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("DC ").Append(DC).Append(" ").Append(SavingThrowType).Append(" Saving Throw.");

            if (FailSavingThrowOutcome.FailSaveType != FailSavingThrowOutcomes.Nothing)
            {
                builder.Append(" The target");
                if (FailSavingThrowOutcome.Damage != Dice.None)
                {
                    builder.Append(" takes ").Append(FailSavingThrowOutcome.Damage).Append(" ").Append(FailSavingThrowOutcome.DamageType);
                    if (FailSavingThrowOutcome.FailSaveType == FailSavingThrowOutcomes.DamageAndCondition)
                    {
                        builder.Append(" and ");
                    }
                }
                if (FailSavingThrowOutcome.Condition != ConditionTypes.none)
                {
                    builder.Append(" becomes ").Append(FailSavingThrowOutcome.Condition);
                }
                builder.Append(" on a failed save");

                if (SuccessSavingThrowOutcome.SuccessSaveType != SuccessSavingThrowOutcomes.Nothing)
                {
                    builder.Append(", or ");

                    if ((FailSavingThrowOutcome.FailSaveType == FailSavingThrowOutcomes.Damage ||
                         FailSavingThrowOutcome.FailSaveType == FailSavingThrowOutcomes.DamageAndCondition) &&
                         SuccessSavingThrowOutcome.SuccessSaveType == SuccessSavingThrowOutcomes.Half)
                    {
                        builder.Append(" or half as much damage ");
                    }
                    builder.Append("on a successful one.");
                }
                builder.Append(".");
            }
            if (!string.IsNullOrEmpty(FailSavingThrowOutcome.OutcomeNote))
            {
                builder.Append(" ").Append(FailSavingThrowOutcome.OutcomeNote);
            }
            return builder.ToString();
        }

        public static SavingThrow Copy(SavingThrow savingThrow, int newDC)
        {
            return savingThrow.IsGrapple
                ? new SavingThrow(savingThrow.Name, false, newDC, savingThrow.FailSavingThrowOutcome, savingThrow.SuccessSavingThrowOutcome)
                : new SavingThrow(savingThrow.Name, savingThrow.SavingThrowType, newDC, savingThrow.FailSavingThrowOutcome, savingThrow.SuccessSavingThrowOutcome);
        }

    }
}