using SummonsTracker.Licensing;
using SummonsTracker.Loading;
using SummonsTracker.Rolling;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Characters
{

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData", order = 1)]
    public class CharacterData : LicensedScriptableObject, ICharacterData, ILoadable<CharacterData>
    {
        public static IReadOnlyList<CharacterData> AllCharacters => _allCharacters;
        private static List<CharacterData> _allCharacters = new List<CharacterData>();

        public string Name => _name;
        public CreatureType Creature => _creature;

        public Movement[] Movement => _movement;

        public Dice MaxHP => _maxHP;
        public int AC => _ac;

        public int Proficiency => _proficiency;
        public int Strength => _strength;
        public int Dexterity => _dexterity;
        public int Constitution => _constitution;
        public int Intelligence => _intelligence;
        public int Wisdom => _wisdom;
        public int Charisma => _charisma;

        public Skills Skills => _skills;
        public StatType SavingThrows => _savingThrows;
        public ConditionTypes ConditionImmunities => _conditionImmunities;
        public DamageTypes DamageVulnerabilities => _damageVulnerabilities;
        public DamageTypes DamageResistances => _damageResistances;
        public DamageTypes DamageImmunities => _damageImmunities;

        public string Senses => _senses;
        public string Languages => _languages;
        public float Challenge => _challenge;

        public ActionData[] Actions => _actions;

        public int StrengthMod => CharacterHelper.GetMod(Strength);
        public int DexterityMod => CharacterHelper.GetMod(Dexterity);
        public int ConstitutionMod => CharacterHelper.GetMod(Constitution);
        public int IntelligenceMod => CharacterHelper.GetMod(Intelligence);
        public int WisdomMod => CharacterHelper.GetMod(Wisdom);
        public int CharismaMod => CharacterHelper.GetMod(Charisma);

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


        [SerializeField]
        private string _name;
        [SerializeField]
        private CreatureType _creature = CreatureType.Humanoid;
        [SerializeField]
        private Movement[] _movement = new[] { new Movement(MovementTypes.Walk, 30) };
        [SerializeField]
        private Dice _maxHP;
        [SerializeField]
        private int _ac;
        [SerializeField]
        private int _proficiency = 2;
        [SerializeField]
        private int _strength = 10;
        [SerializeField]
        private int _dexterity = 10;
        [SerializeField]
        private int _constitution = 10;
        [SerializeField]
        private int _intelligence = 10;
        [SerializeField]
        private int _wisdom = 10;
        [SerializeField]
        private int _charisma = 10;
        [SerializeField]
        private Skills _skills;
        [SerializeField]
        private StatType _savingThrows = StatType.none;
        [SerializeField]
        private ConditionTypes _conditionImmunities = ConditionTypes.none;
        [SerializeField]
        private DamageTypes _damageVulnerabilities = DamageTypes.none;
        [SerializeField]
        private DamageTypes _damageResistances = DamageTypes.none;
        [SerializeField]
        private DamageTypes _damageImmunities = DamageTypes.none;
        [SerializeField]
        public string _senses;
        [SerializeField]
        public string _languages;
        [SerializeField]
        public float _challenge;
        [SerializeField]
        private ActionData[] _actions;

        void ILoadable.Load()
        {
            _allCharacters.Add(this);
        }

        int ILoadable.Priority => 0;
    }
}