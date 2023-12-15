using SummonsTracker.Save;
using System;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public class Character
    {
        public ICharacterData CharacterData;
        public string Name;
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

        public Action[] Actions;

        public int Hitpoints;
        public int TemporaryHitPoints;

        public int StrengthMod => GetMod(Strength);
        public int DexterityMod => GetMod(Dexterity);
        public int ConstitutionMod => GetMod(Constitution);
        public int IntelligenceMod => GetMod(Intelligence);
        public int WisdomMod => GetMod(Wisdom);
        public int CharismaMod => GetMod(Charisma);

        public ConditionTypes Conditions { get; private set; }

        public Character(SaveCharacter saveCharacter, ICharacterData characterData)
        {
            Debug.Log(saveCharacter);
            CharacterData = characterData;

            Name = saveCharacter.Name;
            Hitpoints = saveCharacter.Hitpoints;
            MaxHP = saveCharacter.MaxHP;
            TemporaryHitPoints = saveCharacter.TemporaryHitPoints;
            Conditions = saveCharacter.Conditions;

            Creature = saveCharacter.Creature;
            Movement = saveCharacter.Movement;
            AC = saveCharacter.AC;
            Proficiency = saveCharacter.Proficiency;
            Strength = saveCharacter.Strength;
            Dexterity = saveCharacter.Dexterity;
            Constitution = saveCharacter.Constitution;
            Intelligence = saveCharacter.Intelligence;
            Wisdom = saveCharacter.Wisdom;
            Charisma = saveCharacter.Charisma;
            Skills = saveCharacter.Skills;
            SavingThrows = saveCharacter.SavingThrows;
            ConditionImmunities = saveCharacter.ConditionImmunities;
            DamageVulnerabilities = saveCharacter.DamageVulnerabilities;
            DamageResistances = saveCharacter.DamageResistances;
            DamageImmunities = saveCharacter.DamageImmunities;

            if (saveCharacter.Actions == null || saveCharacter.Actions.Length == 0)
            {
                Actions = Array.Empty<Action>();
            }
            else
            {
                Actions = new Action[saveCharacter.Actions.Length];
                for (int i = 0; i < saveCharacter.Actions.Length; i++)
                {
                    Actions[i] = SaveParser.Convert(saveCharacter.Actions[i]);
                }
            }
        }

        public Character(CharacterData characterData, bool useHPAverage = false)
        {
            CharacterData = characterData;

            Name = characterData.Name;
            Creature = characterData.Creature;
            Movement = characterData.Movement;
            MaxHP = useHPAverage ? characterData.MaxHP.Average : characterData.MaxHP.Roll();
            AC = characterData.AC;
            Proficiency = characterData.Proficiency;
            Strength = characterData.Strength;
            Dexterity = characterData.Dexterity;
            Constitution = characterData.Constitution;
            Intelligence = characterData.Intelligence;
            Wisdom = characterData.Wisdom;
            Charisma = characterData.Charisma;
            Skills = characterData.Skills;
            SavingThrows = characterData.SavingThrows;
            ConditionImmunities = characterData.ConditionImmunities;
            DamageVulnerabilities = characterData.DamageVulnerabilities;
            DamageResistances = characterData.DamageResistances;
            DamageImmunities = characterData.DamageImmunities;

            Actions = new Action[characterData.Actions.Length];
            for (int i = 0; i < characterData.Actions.Length; i++)
            {
                Actions[i] = characterData.Actions[i].Instantiate();
            }

            Hitpoints = MaxHP;
        }

        public int GetStat(StatType stat)
        {
            var total = 0;
            if ((stat & StatType.Strength) != 0)
            {
                total += Strength;
            }
            if ((stat & StatType.Dexterity) != 0)
            {
                total += Dexterity;
            }
            if ((stat & StatType.Constitution) != 0)
            {
                total += Constitution;
            }
            if ((stat & StatType.Intelligence) != 0)
            {
                total += Intelligence;
            }
            if ((stat & StatType.Wisdom) != 0)
            {
                total += Wisdom;
            }
            if ((stat & StatType.Charisma) != 0)
            {
                total += Charisma;
            }
            return total;
        }

        public int GetStatMod(StatType stat)
        {
            var total = 0;
            if ((stat & StatType.Strength) != 0)
            {
                total += StrengthMod;
            }
            if ((stat & StatType.Dexterity) != 0)
            {
                total += DexterityMod;
            }
            if ((stat & StatType.Constitution) != 0)
            {
                total += ConstitutionMod;
            }
            if ((stat & StatType.Intelligence) != 0)
            {
                total += IntelligenceMod;
            }
            if ((stat & StatType.Wisdom) != 0)
            {
                total += WisdomMod;
            }
            if ((stat & StatType.Charisma) != 0)
            {
                total += CharismaMod;
            }
            return total;
        }

        public void Heal(int healAmount)
        {
            Hitpoints = Mathf.Min(Hitpoints + healAmount, MaxHP);
        }
        public void AddTemporaryHitPoints(int temporaryHitpoints)
        {
            TemporaryHitPoints = Mathf.Max(temporaryHitpoints, TemporaryHitPoints);
        }

        public bool TakeDamage(DamageTypes damageType, int damage)
        {
            var amt = damage;
            if (IsImmune(damageType))
            {
                return false;
            }
            if (IsVulnerable(damageType))
            {
                amt *= 2;
            }
            if (IsResistant(damageType))
            {
                amt = Mathf.FloorToInt(amt * 0.5f);
            }
            return DeductHitPoints(amt);
        }

        public bool DeductHitPoints(int damage)
        {
            if (TemporaryHitPoints > 0)
            {
                if (TemporaryHitPoints >= damage)
                {
                    TemporaryHitPoints -= damage;
                    damage = 0;
                }
                else
                {
                    damage -= TemporaryHitPoints;
                    TemporaryHitPoints = 0;
                }
            }

            Hitpoints -= damage;

            if (Hitpoints < 0)
            {
                Hitpoints = 0;
            }
            return Hitpoints == 0;
        }

        public bool IsResistant(DamageTypes damageType)
        {
            return (damageType & DamageResistances) != 0;
        }

        public bool IsImmune(DamageTypes damageType)
        {
            return (damageType & DamageImmunities) != 0;
        }

        public bool IsVulnerable(DamageTypes damageType)
        {
            return (damageType & DamageVulnerabilities) != 0;
        }

        public void SetCondition(ConditionTypes condition)
        {
            if (!IsImmune(condition))
            {
                Conditions |= condition;
            }
        }

        public bool IsImmune(ConditionTypes condition)
        {
            return (condition & ConditionImmunities) != 0;
        }

        public void RemoveCondition(ConditionTypes condition)
        {
            var pre = Conditions.ToString();
            Conditions &= ~condition;
            Debug.Log($"{pre}\n{Conditions}");
        }

        public static int GetMod(int score) => Mathf.FloorToInt((score - 10) * 0.5f);

    }
}