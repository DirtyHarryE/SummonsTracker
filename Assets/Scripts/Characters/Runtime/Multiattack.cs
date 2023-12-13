using SummonsTracker.Multiattacks;

namespace SummonsTracker.Characters
{
    public class Multiattack : Action
    {
        public MultiattackInfo[] Attacks;
        public Multiattack(MultiattackData multiattackData) : base(multiattackData)
        {
            Attacks = new MultiattackInfo[multiattackData.Attacks.Length];
            for (int i = 0; i < multiattackData.Attacks.Length; i++)
            {
                Attacks[i] = new MultiattackInfo(multiattackData.Attacks[i].AttackIndex, multiattackData.Attacks[i].AttackNumber);
            }
        }

        public Multiattack(string name, string note, MultiattackInfo[] attacks) : base(name, note)
        {
            Attacks = attacks;
        }

        public class MultiattackInfo : IMultiattack
        {
            public bool AnyAttack { get; private set; }
            public int AttackIndex { get; private set; }
            public int AttackNumber { get; set; }

            public MultiattackInfo(int attackIndex, int attackNumber)
            {
                AnyAttack = attackIndex <= 0;
                AttackIndex = attackIndex;
                AttackNumber = attackNumber;
            }
        }
    }
}