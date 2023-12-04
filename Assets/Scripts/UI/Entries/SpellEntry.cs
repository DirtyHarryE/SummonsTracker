using SummonsTracker.Spell;
using SummonsTracker.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class SpellEntry : MonoBehaviour
    {
        public void Initialise(SpellData spellData, SpellsPanel spellsPanel, int index)
        {
            _spellsPanel = spellsPanel;
            _index = index;

            _spellName.text = TextUtils.DeCamelCase(spellData.name);
            _spellLevel.text = spellData.Level.ToString();
            _concentrationIcon.SetActive(spellData.Concentration);
        }
        public void SelectEntry()
        {
            _spellsPanel.SelectSpell(_index);
        }

        [SerializeField]
        private TextMeshProUGUI _spellName;
        [SerializeField]
        private TextMeshProUGUI _spellLevel;
        [SerializeField]
        private GameObject _concentrationIcon;

        private SpellsPanel _spellsPanel;
        private int _index;
    }
}