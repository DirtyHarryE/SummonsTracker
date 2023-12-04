using SummonsTracker.Rolling;
using System.Collections.Generic;
using System.Linq;

namespace SummonsTracker.Characters
{
    public struct CharacterActionInfo
    {
        public struct CharacterAttackInfo
        {
            public readonly AdvantageType AdvantageType;
            public readonly Action Action;

            public CharacterAttackInfo(AdvantageType advantageType, Action action)
            {
                AdvantageType = advantageType;
                Action = action;
            }
            public static implicit operator CharacterAttackInfo(Action a ) => new CharacterAttackInfo(AdvantageType.None, a);
        }
        public readonly Character Character;
        public readonly IEnumerable<CharacterAttackInfo> Actions;

        public CharacterActionInfo(Character character, IEnumerable<CharacterAttackInfo> actions)
        {
            Character = character;
            Actions = actions;
        }
        public CharacterActionInfo(Character character, IEnumerable<Action> actions)
        {
            Character = character;
            Actions = actions.Select(a => new CharacterAttackInfo(AdvantageType.None, a));
        }
    }
}