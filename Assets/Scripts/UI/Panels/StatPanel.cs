using UnityEngine;

namespace SummonsTracker.UI
{
    public class StatPanel : Panel
    {
        public int WizardLevel => _wizardLevel.Score;
        public int SpellcastingAbility => _spellcastingAbility.Score;
        public int SpellAtkMod => _spellAtkMod.Score;
        public int SpellSaveDC => _spellSave.Score;
        public int Proficiency => _proficiency.Score;
        public int HpMax => _hpMax.Score;
        public int Strength => _strength.Score;
        public int Dexterity => _dexterity.Score;
        public int Constitution => _constitution.Score;
        public int Intelligence => _intelligence.Score;
        public int Wisdom => _wisdom.Score;
        public int Charisma => _charisma.Score;

        [Header("Stats")]
        [SerializeField]
        private StatBox _wizardLevel;
        [SerializeField]
        private StatBox _spellcastingAbility;
        [SerializeField]
        private StatBox _spellAtkMod;
        [SerializeField]
        private StatBox _spellSave;
        [SerializeField]
        private StatBox _proficiency;
        [SerializeField]
        private StatBox _hpMax;
        [SerializeField]
        private StatBox _strength;
        [SerializeField]
        private StatBox _dexterity;
        [SerializeField]
        private StatBox _constitution;
        [SerializeField]
        private StatBox _intelligence;
        [SerializeField]
        private StatBox _wisdom;
        [SerializeField]
        private StatBox _charisma;
    }
}