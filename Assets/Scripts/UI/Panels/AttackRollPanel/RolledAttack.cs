using SummonsTracker.Characters;
using SummonsTracker.Rolling;

namespace SummonsTracker.UI
{
    public struct RolledAttack
    {
        public readonly string AttackInstanceGUID;
        public int Result => D20Roll + Attack.AttackMod;
        public bool IsCrit => D20Roll >= 20;

        public int D20Roll;
        public AdvantageType Advantage;
        public Attack Attack;
        public Character Character;

        public int[] DamageRollResults;

        public int AttackIndex;

        public RolledAttack(int d20Roll, AdvantageType advantage, Attack attack, Character character, int[] damageRollResults, int attackIndex)
        {
            AttackInstanceGUID = System.Guid.NewGuid().ToString();
            D20Roll = d20Roll;
            Advantage = advantage;
            Attack = attack;
            Character = character;
            DamageRollResults = damageRollResults;
            AttackIndex = attackIndex;
        }
    }
}