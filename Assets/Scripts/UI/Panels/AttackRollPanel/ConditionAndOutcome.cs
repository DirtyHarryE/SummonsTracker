using SummonsTracker.Characters;
using SummonsTracker.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.UI
{
    public struct ConditionAndOutcome
    {
        public readonly SaveTarget Target;
        public readonly ConditionTypes Condition;
        public readonly string Outcome;

        public ConditionAndOutcome(SaveTarget target, ConditionTypes condition, string outcome)
        {
            Target = target;
            Condition = condition;
            Outcome = outcome;
        }

        public static bool operator ==(ConditionAndOutcome a, ConditionAndOutcome b)
        {
            return a.Target == b.Target && a.Condition == b.Condition && a.Outcome == b.Outcome;
        }

        public static bool operator !=(ConditionAndOutcome a, ConditionAndOutcome b)
        {
            return a.Target != b.Target && a.Condition != b.Condition || a.Outcome != b.Outcome;
        }

        public override bool Equals(object obj)
        {
            if (obj is ConditionAndOutcome other)
            {
                return Target == other.Target && Condition == other.Condition && Outcome == other.Outcome;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Target.GetHashCode();
                hash = hash * 31 + Condition.GetHashCode();
                hash = hash * 31 + Outcome.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"{Target}:{Condition}:{Outcome}";
        }
    }
}