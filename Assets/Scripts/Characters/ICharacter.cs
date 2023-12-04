using SummonsTracker.Rolling;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public interface ICharacter
    {
        string Name { get; }
        CreatureType Creature { get; }

        Movement[] Movement { get; }

        Rolling.Dice MaxHP { get; }
        int AC { get; }

        int Proficiency { get; }
        int Strength { get; }
        int Dexterity { get; }
        int Constitution { get; }
        int Intelligence { get; }
        int Wisdom { get; }
        int Charisma { get; }

        Skills Skills { get; }
        StatType SavingThrows { get; }
        ConditionTypes ConditionImmunities { get; }
        DamageTypes DamageVulnerabilities { get; }
        DamageTypes DamageResistances { get; }
        DamageTypes DamageImmunities { get; }

        ActionData[] Actions { get; }
    }
}