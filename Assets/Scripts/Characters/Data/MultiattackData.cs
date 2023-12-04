using SummonsTracker.Multiattacks;
using UnityEngine;
namespace SummonsTracker.Characters
{
    [ActionMenuItem("Multiattack")]
    public class MultiattackData : ActionData
    {
        public MultiattackEntry[] Attacks => _attacks;
        [SerializeField]
        private MultiattackEntry[] _attacks;

        [System.Serializable]
        public class MultiattackEntry : IMultiattack
        {
            [SerializeField]
            private int _attackIndex;
            [SerializeField]
            private int _attackNumber;

            public int AttackIndex => _attackIndex;
            public int AttackNumber => _attackNumber;
        }

        public override Action Instantiate()
        {
            return new Multiattack(this);
        }
    }
}