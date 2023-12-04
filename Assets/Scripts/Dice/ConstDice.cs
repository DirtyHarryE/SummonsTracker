using UnityEngine;

namespace SummonsTracker.Rolling
{
    public class ConstDice : DiceBase
    {
        public override bool IsActive => true;
        public override int Faces => 0;
        public override int Number => 0;
        public override int Modifiers => _value;

        public override int Roll(AdvantageType advantageType = AdvantageType.None) => _value;
        public override int Average => _value;

        public ConstDice(int value)
        {
            _value = value;
        }

        public static implicit operator ConstDice(int dice) => new ConstDice(dice);

        [SerializeField]
        private int _value;
    }
}