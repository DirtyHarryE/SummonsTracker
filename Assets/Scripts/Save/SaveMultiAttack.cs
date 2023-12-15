using System;

namespace SummonsTracker.Save
{
    [Serializable]
    public class SaveMultiAttack
    {
        public bool AnyAttack { get; private set; }
        public int AttackIndex { get; private set; }
        public int AttackNumber { get; set; }

        public SaveMultiAttack(int attackIndex, int attackNumber)
        {
            AnyAttack = attackIndex <= 0;
            AttackIndex = attackIndex;
            AttackNumber = attackNumber;
        }

        public override string ToString()
        {
            return $"{(AnyAttack ? "Any" : AttackIndex.ToString())} => {AttackNumber}";
        }
    }
}