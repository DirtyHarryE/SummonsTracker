using SummonsTracker.Rolling;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public static class SavingThrowHelper
    {
        public static string SavingThrowToString(this ISavingThrow savingThrow)
        {
            var builder = new StringBuilder();
            builder.Append("DC ").Append(savingThrow.DC).Append(" ").Append(savingThrow.SavingThrow).Append(" Saving Throw.");

            if (savingThrow.FailureSavingThrowOutcome.FailSaveType != FailSavingThrowOutcomes.Nothing)
            {
                builder.Append(" The target");
                if (savingThrow.FailureSavingThrowOutcome.Damage != Dice.None)
                {
                    builder.Append(" takes ").Append(savingThrow.FailureSavingThrowOutcome.Damage).Append(" ").Append(savingThrow.FailureSavingThrowOutcome.DamageType);
                    if (savingThrow.FailureSavingThrowOutcome.FailSaveType == FailSavingThrowOutcomes.DamageAndCondition)
                    {
                        builder.Append(" and ");
                    }
                }
                if (savingThrow.FailureSavingThrowOutcome.Condition != ConditionTypes.none)
                {
                    builder.Append(" becomes ").Append(savingThrow.FailureSavingThrowOutcome.Condition);
                }
                builder.Append(" on a failed save");

                if (savingThrow.SuccessSavingThrowOutcome.SuccessSaveType != SuccessSavingThrowOutcomes.Nothing)
                {
                    builder.Append(", or ");

                    if ((savingThrow.FailureSavingThrowOutcome.FailSaveType == FailSavingThrowOutcomes.Damage ||
                         savingThrow.FailureSavingThrowOutcome.FailSaveType == FailSavingThrowOutcomes.DamageAndCondition) &&
                         savingThrow.SuccessSavingThrowOutcome.SuccessSaveType == SuccessSavingThrowOutcomes.Half)
                    {
                        builder.Append(" or half as much damage ");
                    }
                    else
                    {
                        if (savingThrow.SuccessSavingThrowOutcome.Damage != Dice.None)
                        {
                            builder.Append(" takes ").Append(savingThrow.SuccessSavingThrowOutcome.Damage).Append(" ").Append(savingThrow.SuccessSavingThrowOutcome.DamageType);
                            if (savingThrow.SuccessSavingThrowOutcome.SuccessSaveType == SuccessSavingThrowOutcomes.DamageAndCondition)
                            {
                                builder.Append(" and ");
                            }
                        }
                        if (savingThrow.SuccessSavingThrowOutcome.Condition != ConditionTypes.none)
                        {
                            builder.Append(" becomes ").Append(savingThrow.SuccessSavingThrowOutcome.Condition);
                        }
                    }
                    builder.Append("on a successful one");
                }
                builder.Append(".");
            }
            if (!string.IsNullOrEmpty(savingThrow.FailureSavingThrowOutcome.OutcomeNote))
            {
                builder.Append(" ").Append(savingThrow.FailureSavingThrowOutcome.OutcomeNote);
            }
            return builder.ToString();
        }

    }
}