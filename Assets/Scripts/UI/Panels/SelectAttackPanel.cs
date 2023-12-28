using SummonsTracker.Characters;
using SummonsTracker.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public class SelectAttackPanel : Panel
    {
        public void Initialise(IEnumerable<Character> characters)
        {
            _allEntries.Clear();
            _characterEntries.Clear();
            _subEntries.Clear();
            ClearEntries();
            foreach (var group in characters.GroupBy(c => c.CharacterData))
            {
                var topName = $"{group.Key.Name}s \u00D7 {group.Count()}";
                var data = group.Key;

                AttackEntry firstEntry = null;
                var subEntries = new List<AttackEntry>();
                var moreThan1 = group.Count() > 1;
                if (moreThan1)
                {
                    var first = group.First();
                    firstEntry = MakeAttackEntry(first, topName);
                    var topLine = GameObject.Instantiate(_linePrefab, _content);
                    topLine.SetActive(false);
                    _allEntries.Add(firstEntry);
                    firstEntry.SetExpandAction(delegate
                    {
                        var show = !topLine.activeInHierarchy;
                        foreach (var other in _subEntries[firstEntry])
                        {
                            other.gameObject.SetActive(show);
                        }
                        topLine.SetActive(show);
                        UpdateBackgrounds();
                        LayoutRebuilder.ForceRebuildLayoutImmediate(_content.transform as RectTransform);
                    });

                    firstEntry.ShowExpandButton(true);
                }
                foreach (var c in group)
                {
                    var e = MakeAttackEntry(c, c.Name);
                    e.ShowExpandButton(false);
                    e.gameObject.SetActive(!moreThan1);
                    subEntries.Add(e);
                    _allEntries.Add(e);
                    _characterEntries.Add(e);
                }
                if (firstEntry != null)
                {
                    firstEntry.OnValueChanged = v => subEntries.ForEach(e => e.SetValue(v));
                    firstEntry.OnAdvantageChanged = a => subEntries.ForEach(e => e.SetAdvantage(a));
                    firstEntry.OnTargetChanged = t => subEntries.ForEach(e => e.ChangeTarget(t));
                    _subEntries.Add(firstEntry, subEntries.ToArray());
                }
                GameObject.Instantiate(_linePrefab, _content);
            }
            this.Open();
            UpdateBackgrounds();
        }

        public void DoAttack()
        {
                var infos = _characterEntries.Select(e => new CharacterActionInfo(e.Character, GetCharacterAttackInfo(e)));
                GameManager.Instance.AttackRollPanel.Initialise(infos);
        }

        private void UpdateBackgrounds()
        {
            var odd = false;
            foreach (var e in _allEntries)
            {
                if (e.gameObject.activeInHierarchy)
                {
                    e.EnableBackground(odd);
                    odd = !odd;
                }
            }
        }

        private IEnumerable<CharacterActionInfo.CharacterAttackInfo> GetCharacterAttackInfo(AttackEntry entry)
        {
            foreach (var a in entry.GetActions())
            {
                yield return new CharacterActionInfo.CharacterAttackInfo(entry.GetTarget(), entry.GetAdvantage(), a);
            }
        }

        private void OnDisable()
        {
            ClearEntries();
        }

        private void ClearEntries()
        {
            var childCount = _content.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var t = _content.GetChild(i);
                if (t != null)
                {
                    UnityEngine.Object.Destroy(t.gameObject);
                }
            }
        }

        private AttackEntry MakeAttackEntry(Character character, string title)
        {
            AttackEntry entry;
            var attacks = 0;
            var multiattacks = 0;
            foreach (var action in character.Actions)
            {
                if (action is Multiattack multiattack)
                {
                    multiattacks += multiattack.Attacks.Sum(a => a.AttackNumber);
                }
                else
                {
                    attacks += 1;
                }
            }
            if (multiattacks > 1)
            {
                var go = GameObject.Instantiate(_attackMultiSelectEntryPrefab, _content);
                entry = go.GetComponent<AttackMultiSelectEntry>();
            }
            else if (attacks > 1)
            {
                var go = GameObject.Instantiate(_attackSingleSelectEntryPrefab, _content);
                entry = go.GetComponent<AttackSingleSelectEntry>();
            }
            else
            {
                var go = GameObject.Instantiate(_preAttackTextPrefab, _content);
                entry = go.GetComponent<PresetAttackEntry>();
            }

            entry.name = title;
            entry.Initialise(character, title, onNewTarget =>
            {
                _createTargetPanel.Init(onNewTarget);
                _createTargetPanel.Open();
            });
            return entry;
        }

        private List<AttackEntry> _allEntries = new List<AttackEntry>();
        private List<AttackEntry> _characterEntries = new List<AttackEntry>();
        private Dictionary<AttackEntry, AttackEntry[]> _subEntries = new Dictionary<AttackEntry, AttackEntry[]>();

        [SerializeField, FormerlySerializedAs("_attackEntryPrefab")]
        private GameObject _attackSingleSelectEntryPrefab;
        [SerializeField]
        private GameObject _attackMultiSelectEntryPrefab;
        [SerializeField]
        private GameObject _linePrefab;
        [SerializeField]
        private GameObject _preAttackTextPrefab;
        [SerializeField]
        private Transform _content;
        [Space]
        [SerializeField]
        private CreateTargetPanel _createTargetPanel;
    }
}