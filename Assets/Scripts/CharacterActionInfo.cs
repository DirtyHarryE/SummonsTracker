using SummonsTracker.Rolling;
using SummonsTracker.Save;
using System.Collections.Generic;
using System.Linq;

namespace SummonsTracker.Characters
{
    public struct CharacterActionInfo
    {
        public struct CharacterAttackInfo
        {
            public readonly SaveTarget Target;
            public readonly AdvantageType AdvantageType;
            public readonly Action Action;

            public CharacterAttackInfo(SaveTarget target, AdvantageType advantageType, Action action)
            {
                Target = target;
                AdvantageType = advantageType;
                Action = action;
            }
            public static implicit operator CharacterAttackInfo(Action a ) => new CharacterAttackInfo(SaveTarget.None, AdvantageType.None, a);
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
            Actions = actions.Select(a => new CharacterAttackInfo(SaveTarget.None, AdvantageType.None, a));
        }
    }
}