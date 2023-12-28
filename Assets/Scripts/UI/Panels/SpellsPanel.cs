using SummonsTracker.Spell;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class SpellsPanel : Panel
    {
        protected override void Awake()
        {
            _spells = SpellData.AllSpells.OrderBy(s => s.Level).ThenBy(s => s.name).ToArray();
            base.Awake();
        }

        public void SelectSpell(int index)
        {
            _castSpellPanel.SelectSpell(_spells[index]);
        }

        private void Start()
        {
            for (int i = 0; i < _spells.Length; i++)
            {
                var instGO = GameObject.Instantiate(_spellEntryPrefab, _parent);
                var entry = instGO.GetComponent<SpellEntry>();
                entry.Initialise(_spells[i], this, i);
            }
        }

        private SpellData[] _spells;

        [SerializeField]
        private CastSpellPanel _castSpellPanel;

        [SerializeField]
        private GameObject _spellEntryPrefab;

        [SerializeField]
        private RectTransform _parent;
    }
}