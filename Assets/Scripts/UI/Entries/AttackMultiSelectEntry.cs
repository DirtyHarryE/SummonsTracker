using SummonsTracker.Characters;
using SummonsTracker.Multiattacks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class AttackMultiSelectEntry : AttackEntry
    {
        private struct MultiAttackNumbers : IMultiattack
        {
            public int AttackIndex { get; private set; }
            public int AttackNumber { get; private set; }

            public MultiAttackNumbers(int attackIndex, int attackNumber)
            {
                AttackIndex = attackIndex;
                AttackNumber = attackNumber;
            }
        }
        public override void Initialise(Character character, string title)
        {
            base.Initialise(character, title);
            var buttonIndex = 0;

            for (int i = 0; i < character.Actions.Length; i++)
            {
                if (character.Actions[i] is Multiattack multiattack)
                {
                    _multiattacks.Add(multiattack);
                }
                else
                {
                    _multiAttackButtons.Add(InitialiseMultiButton(character.Actions[i].Name, i, buttonIndex++));
                }
            }
            foreach (var m in _multiAttackButtons)
            {
                m.AttackNumber = 0;
            }

            var firstMulti = _multiattacks.First();
            foreach (var atk in firstMulti.Attacks)
            {
                if (atk.AnyAttack)
                {
                    _multiAttackButtons.First().AttackNumber = atk.AttackNumber;
                }
                else
                {
                    foreach (var mButton in _multiAttackButtons)
                    {
                        if (mButton.AttackIndex == atk.AttackIndex)
                        {
                            mButton.AttackNumber = atk.AttackNumber;
                        }
                    }
                }
            }
        }

        public SelectMultiAttackButton InitialiseMultiButton(string text, int attackIndex, int buttonIndex)
        {
            _row.gameObject.SetActive(true);
            var instGO = GameObject.Instantiate(_multiButtonPrefab, _row);
            var button = instGO.GetComponent<SelectMultiAttackButton>();

            button.Initialise(ValidateMultiattack, ProcessMultiAttackButton, text, attackIndex, buttonIndex);

            return button;
        }

        public override object GetValue()
        {
            return _multiAttackButtons.OfType<IMultiattack>().ToArray();
        }

        public override void SetValue(object obj, bool update = true)
        {
            if (update)
            {
                if (obj is IMultiattack[] multArr)
                {
                    foreach (var mButton in _multiAttackButtons)
                    {
                        foreach (var m in multArr)
                        {
                            if (mButton.AttackIndex == m.AttackIndex && mButton.AttackNumber != m.AttackNumber)
                            {
                                mButton.SetNumberDirect(m.AttackNumber);
                            }
                        }
                    }
                }
                foreach (var mButton in _multiAttackButtons)
                {
                    mButton.Validate();
                }
            }
        }

        public override IEnumerable<Action> GetActions()
        {
            foreach (var multiattack in _multiAttackButtons)
            {
                for (int i = 0; i < multiattack.AttackNumber; i++)
                {
                    yield return Character.Actions[multiattack.AttackIndex];
                }
            }
        }
        private void ProcessMultiAttackButton(SelectMultiAttackButton button, int number)
        {
            foreach (var mButton in _multiAttackButtons)
            {
                if (mButton.ButtonIndex != button.ButtonIndex)
                {
                    mButton.Validate();
                }
            }
            SetValue(new SelectAttackValue(_multiAttackButtons), false);
        }

        private bool ValidateMultiattack(SelectMultiAttackButton button, int number)
        {
            if (number <= 0)
            {
                return true;
            }
            var multiattacks = _multiAttackButtons.Select(m =>
            {
                var mult = (IMultiattack)m;
                if (mult.AttackIndex == button.AttackIndex)
                {
                    mult = new MultiAttackNumbers(button.AttackIndex, number);
                }
                return mult;
            }).ToArray();
            foreach (var multiattack in _multiattacks)
            {
                if (IsMultiattackButton(multiattack, multiattacks))
                {
                    return true;
                }

            }
            return false;
        }

        private bool IsMultiattackButton(Multiattack multiattack, IMultiattack[] selected)
        {
            int allowedTotal = multiattack.Attacks.Sum(a => a.AttackNumber);
            int selectedTotal = selected.Sum(a => a.AttackNumber);
            if (selectedTotal > allowedTotal)
            {
                return false;
            }
            var allowedDict = MultiAttackToDict(multiattack.Attacks);
            var selectedDict = MultiAttackToDict(selected);

            var hasAny = false;
            var anyTotal = 0;
            if (allowedDict.TryGetValue(-1, out var anyTotalGet))
            {
                hasAny = true;
                anyTotal = anyTotalGet;
            }
            foreach (var select in selectedDict)
            {
                var thisSelectedTotal = select.Value;
                if (allowedDict.TryGetValue(select.Key, out var thisAllowedTotal))
                {
                    thisSelectedTotal -= thisAllowedTotal;
                }
                if (hasAny)
                {
                    var toDeduct = thisSelectedTotal;
                    thisSelectedTotal -= anyTotal;
                    anyTotal -= toDeduct;
                    if (anyTotal < 0)
                    {
                        return false;
                    }
                }
                if (thisSelectedTotal > 0)
                {
                    return false;
                }
            }
            return true;
        }

        private Dictionary<int, int> MultiAttackToDict(IMultiattack[] multiattacks)
        {
            var dict = new Dictionary<int, int>();
            foreach (var m in multiattacks)
            {
                if (m.AttackNumber <= 0)
                {
                    continue;
                }
                if (dict.ContainsKey(m.AttackIndex))
                {
                    dict[m.AttackIndex] += m.AttackNumber;
                }
                else
                {
                    dict.Add(m.AttackIndex, m.AttackNumber);
                }
            }
            return dict;
        }

        [Space]
        [SerializeField]
        private GameObject _multiButtonPrefab;

        [SerializeField]
        private RectTransform _row;

        private List<SelectMultiAttackButton> _multiAttackButtons = new List<SelectMultiAttackButton>();
        private List<Multiattack> _multiattacks = new List<Multiattack>();
    }
}