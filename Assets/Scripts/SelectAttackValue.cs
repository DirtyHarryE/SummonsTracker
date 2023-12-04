using SummonsTracker.Multiattacks;
using System.Collections.Generic;

namespace SummonsTracker.UI
{
    public struct SelectAttackValue
    {
        public int SingleAttackIndex;
        public List<IMultiattack> MultiAttacks;

        public SelectAttackValue(int attackIndex) 
        {
            SingleAttackIndex = attackIndex;
            MultiAttacks = new List<IMultiattack>();
        }
        public SelectAttackValue(IEnumerable<IMultiattack> multiAttacks)
        {
            SingleAttackIndex = -1;
            MultiAttacks = new List<IMultiattack>(multiAttacks);
        }
        public static implicit operator SelectAttackValue(int i) => new SelectAttackValue(i);
    }
}