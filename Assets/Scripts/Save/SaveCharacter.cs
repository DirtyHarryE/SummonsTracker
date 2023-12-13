using SummonsTracker.Characters;
using System;
using System.Linq;
using System.Text;

namespace SummonsTracker.Save
{
    [Serializable]
    public class SaveCharacter : ICharacterData
    {
        public string Name;

        public int Hitpoints;
        public int TemporaryHitPoints;
        public ConditionTypes Conditions;
        public bool Concentration;

        public string DataName;

        public CreatureType Creature;

        public Movement[] Movement = new[] { new Movement(MovementTypes.Walk, 30) };

        public int MaxHP;
        public int AC;

        public int Proficiency;

        public int Strength;
        public int Dexterity;
        public int Constitution;
        public int Intelligence;
        public int Wisdom;
        public int Charisma;

        public Skills Skills;

        public StatType SavingThrows = StatType.none;
        public ConditionTypes ConditionImmunities = ConditionTypes.none;
        public DamageTypes DamageVulnerabilities = DamageTypes.none;
        public DamageTypes DamageResistances = DamageTypes.none;
        public DamageTypes DamageImmunities = DamageTypes.none;

        public SaveAction[] Actions;


        public SaveCharacter(Character character, bool concentration) : this(name: character.Name,
                                                                             hitpoints: character.Hitpoints,
                                                                             temporaryHitPoints: character.TemporaryHitPoints,
                                                                             conditions: character.Conditions,
                                                                             concentration: concentration,
                                                                             dataName: character.CharacterData.Name,
                                                                             creature: character.Creature,
                                                                             movement: character.Movement,
                                                                             maxHP: character.MaxHP,
                                                                             aC: character.AC,
                                                                             proficiency: character.Proficiency,
                                                                             strength: character.Strength,
                                                                             dexterity: character.Dexterity,
                                                                             constitution: character.Constitution,
                                                                             intelligence: character.Intelligence,
                                                                             wisdom: character.Wisdom,
                                                                             charisma: character.Charisma,
                                                                             skills: character.Skills,
                                                                             savingThrows: character.SavingThrows,
                                                                             conditionImmunities: character.ConditionImmunities,
                                                                             damageVulnerabilities: character.DamageVulnerabilities,
                                                                             damageResistances: character.DamageResistances,
                                                                             damageImmunities: character.DamageImmunities,
                                                                             actions: character.Actions.Select(SaveManager.Convert).ToArray())
        {

        }

        public SaveCharacter(string name,
                             int hitpoints,
                             int temporaryHitPoints,
                             ConditionTypes conditions,
                             bool concentration,
                             string dataName,
                             CreatureType creature,
                             Movement[] movement,
                             int maxHP,
                             int aC,
                             int proficiency,
                             int strength,
                             int dexterity,
                             int constitution,
                             int intelligence,
                             int wisdom,
                             int charisma,
                             Skills skills,
                             StatType savingThrows,
                             ConditionTypes conditionImmunities,
                             DamageTypes damageVulnerabilities,
                             DamageTypes damageResistances,
                             DamageTypes damageImmunities,
                             SaveAction[] actions)
        {
            Name = name;
            Hitpoints = hitpoints;
            TemporaryHitPoints = temporaryHitPoints;
            Conditions = conditions;
            Concentration = concentration;
            DataName = dataName;
            Creature = creature;
            Movement = movement;
            MaxHP = maxHP;
            AC = aC;
            Proficiency = proficiency;
            Strength = strength;
            Dexterity = dexterity;
            Constitution = constitution;
            Intelligence = intelligence;
            Wisdom = wisdom;
            Charisma = charisma;
            Skills = skills;
            SavingThrows = savingThrows;
            ConditionImmunities = conditionImmunities;
            DamageVulnerabilities = damageVulnerabilities;
            DamageResistances = damageResistances;
            DamageImmunities = damageImmunities;
            Actions = actions;
        }

        public void Validate()
        {

        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Name).Append(" (").Append(Creature).Append("), AC = ").Append(AC).Append(", HP = ").Append(Hitpoints);
            if (TemporaryHitPoints > 0)
            {
                builder.Append("+").Append(TemporaryHitPoints);
            }
            builder.Append("/").Append(MaxHP).Append(", Speed = [");
            for (int i = 0; i < Movement.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(", ");
                }
                builder.Append("(").Append(Movement[i].Distance).Append(" ").Append(Movement[i].Type).Append(")");
            }
            builder.Append("], ");
            builder.Append("STR = ").Append(Strength).Append(", ");
            builder.Append("DEX = ").Append(Dexterity).Append(", ");
            builder.Append("CON = ").Append(Constitution).Append(", ");
            builder.Append("INT = ").Append(Intelligence).Append(", ");
            builder.Append("WIS = ").Append(Wisdom).Append(", ");
            builder.Append("CHA = ").Append(Charisma).Append(", ");
            if (SavingThrows != StatType.none) builder.Append("Saving Throws = ").Append(SavingThrows).Append(", ");
            if (Skills != Skills.none) builder.Append("Skills = ").Append(Skills).Append(", ");
            if (DamageVulnerabilities != DamageTypes.none) builder.Append("Damage Vulnerabilities = ").Append(DamageVulnerabilities).Append(", ");
            if (DamageResistances != DamageTypes.none) builder.Append("Damage Resistances = ").Append(DamageResistances).Append(", ");
            if (DamageImmunities != DamageTypes.none) builder.Append("Damage Immunities = ").Append(DamageImmunities).Append(", ");
            if (ConditionImmunities != ConditionTypes.none) builder.Append("Condition Immunities = ").Append(ConditionImmunities).Append(", ");
            builder.Append("Proficiency Bonus = ").Append(Proficiency).Append(", ");
            builder.Append("Actions = [");
            for (int i = 0; i < Actions.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(", ");
                }
                builder.Append("(").Append(Actions[i]).Append(")");
            }
            builder.Append("]");
            return builder.ToString();
        }

        string ICharacterData.Name => Name;
    }
}