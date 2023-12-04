using SummonsTracker.Spell;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class SpellsPanel : Panel
    {
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
                instGO.transform.SetSiblingIndex(_parent.childCount - 2);
            }
        }

        private void Reset()
        {
            GetSpells();
        }

        [ContextMenu("Get Spells")]
        private void GetSpells()
        {
#if UNITY_EDITOR
            _spells = UnityEditor.AssetDatabase.FindAssets("t:SpellData")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .SelectMany(UnityEditor.AssetDatabase.LoadAllAssetsAtPath)
                .OfType<SpellData>()
                .OrderBy(s => s.Level)
                .ToArray();
#endif
        }
        [SerializeField]
        private CastSpellPanel _castSpellPanel;

        [SerializeField]
        private SpellData[] _spells;
        [SerializeField]
        private GameObject _spellEntryPrefab;

        [SerializeField]
        private RectTransform _parent;
    }
}