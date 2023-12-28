using SummonsTracker.Save;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class StatPanel : Panel
    {
        public override void Open()
        {
            _wizardLevel.Score = SaveManager.Instance.CurrentProfile.WizardLevel;
            _spellcastingAbility.Score = SaveManager.Instance.CurrentProfile.SpellcastingAbility;
            _spellAtkMod.Score = SaveManager.Instance.CurrentProfile.SpellAtkMod;
            _spellSave.Score = SaveManager.Instance.CurrentProfile.SpellSaveDC;
            _proficiency.Score = SaveManager.Instance.CurrentProfile.WizardLevel;
            _hpMax.Score = SaveManager.Instance.CurrentProfile.HpMax;
            _strength.Score = SaveManager.Instance.CurrentProfile.Strength;
            _dexterity.Score = SaveManager.Instance.CurrentProfile.Dexterity;
            _constitution.Score = SaveManager.Instance.CurrentProfile.Constitution;
            _intelligence.Score = SaveManager.Instance.CurrentProfile.Intelligence;
            _wisdom.Score = SaveManager.Instance.CurrentProfile.Wisdom;
            _charisma.Score = SaveManager.Instance.CurrentProfile.Charisma;

            for (int i = 0; i < SaveManager.Instance.CurrentProfile.SavedTargets.Length; i++)
            {
                SaveTarget target = SaveManager.Instance.CurrentProfile.SavedTargets[i];
                var instGO = GameObject.Instantiate(_tagetPrefab, _targetHeader.parent);
                instGO.transform.SetSiblingIndex(_targetHeader.GetSiblingIndex() + i + 1);
                var entry = instGO.GetComponent<SaveTargetEntry>();
                entry.Initialise(target);
                _targetEntries.Add(entry);
            }

            base.Open();
        }

        public void OnValueChanged()
        {
            _hasChanged = true;
        }

        public void Save()
        {
            SaveManager.Instance.CurrentProfile.WizardLevel = _wizardLevel.Score;
            SaveManager.Instance.CurrentProfile.SpellcastingAbility = _spellcastingAbility.Score;
            SaveManager.Instance.CurrentProfile.SpellAtkMod = _spellAtkMod.Score;
            SaveManager.Instance.CurrentProfile.SpellSaveDC = _spellSave.Score;
            SaveManager.Instance.CurrentProfile.Proficiency = _proficiency.Score;
            SaveManager.Instance.CurrentProfile.HpMax = _hpMax.Score;
            SaveManager.Instance.CurrentProfile.Strength = _strength.Score;
            SaveManager.Instance.CurrentProfile.Dexterity = _dexterity.Score;
            SaveManager.Instance.CurrentProfile.Constitution = _constitution.Score;
            SaveManager.Instance.CurrentProfile.Intelligence = _intelligence.Score;
            SaveManager.Instance.CurrentProfile.Wisdom = _wisdom.Score;
            SaveManager.Instance.CurrentProfile.Charisma = _charisma.Score;

            SaveManager.Instance.CurrentProfile.SavedTargets =_targetEntries.Where(t => t.TargetEnabled).Select(t => t.Target).ToArray();

            SaveManager.Instance.Save();
        }

        public void Apply()
        {
            if (_hasChanged)
            {
                Save();
            }
            _hasChanged = false;
            Close();
        }

        protected override void OnClose()
        {
            for (int i = 0; i < _targetEntries.Count; i++)
            {
                Object.Destroy(_targetEntries[i].gameObject);
            }
            _targetEntries.Clear();
            base.OnClose();
        }

        private List<SaveTargetEntry> _targetEntries = new List<SaveTargetEntry>();
        private bool _hasChanged;

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
        [Space]
        [SerializeField]
        private Transform _targetHeader;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject _tagetPrefab;
    }
}