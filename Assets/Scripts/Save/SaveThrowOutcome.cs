using SummonsTracker.Characters;
using System;

namespace SummonsTracker.Save
{
    [Serializable]
    public class SaveThrowOutcome
    {
        public string SaveType;
        public SaveDamage Damage;
        public ConditionTypes Condition;
        public string OutcomeNote;

        public SaveThrowOutcome(string saveType, SaveDamage damage, ConditionTypes condition, string outcomeNote)
        {
            SaveType = saveType;
            Damage = damage;
            Condition = condition;
            OutcomeNote = outcomeNote;
        }

        public static SaveThrowOutcome None = new SaveThrowOutcome("none", SaveDamage.None, ConditionTypes.none, string.Empty);

        public override string ToString()
        {
            return $"Save Type = {SaveType}, Damage = {Damage}, Condition = {Condition}, Outcome Note = {OutcomeNote}";
        }
    }
}