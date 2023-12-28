using SummonsTracker.Characters;
using SummonsTracker.Save;

namespace SummonsTracker.UI
{
    public struct RolledSavingThrow
    {
        public readonly string SavingThrowInstanceGUID;

        public ISavingThrow SavingThrow;
        public Character Character;

        public int FailDamageResult;
        public int SuccessDamageResult;

        public int AttackIndex;

        public SaveTarget Target;

        public RolledSavingThrow(string guid, ISavingThrow savingThrow, Character character, int failDamageResult, int successDamageResult, int attackIndex, SaveTarget target)
        {
            SavingThrowInstanceGUID = guid;
            SavingThrow = savingThrow;
            Character = character;
            FailDamageResult = failDamageResult;
            SuccessDamageResult = successDamageResult;
            AttackIndex = attackIndex;
            Target = target;
        }
    }
}