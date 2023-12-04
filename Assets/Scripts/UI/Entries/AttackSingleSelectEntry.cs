using SummonsTracker.Characters;
using SummonsTracker.Multiattacks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public class AttackSingleSelectEntry : AttackEntry
    {
        public override void Initialise(Character character, string title)
        {
            base.Initialise(character, title);
            var buttonIndex = 0;

            for (int i = 0; i < character.Actions.Length; i++)
            {
                if (!(character.Actions[i] is Multiattack))
                {
                    _singleAttackButtons.Add(InitialiseSingleButton(character.Actions[i].Name, i, buttonIndex++));
                }
            }
            ProcessSingleAttackButton(_singleAttackButtons[0]);
        }

        public SelectSingleAttackButton InitialiseSingleButton(string text, int attackIndex, int buttonIndex)
        {
            _row.gameObject.SetActive(true);
            var instGO = GameObject.Instantiate(_singleButtonPrefab, _row);
            var button = instGO.GetComponent<SelectSingleAttackButton>();

            button.Initialise(ProcessSingleAttackButton, text, attackIndex, buttonIndex);

            return button;
        }

        public override object GetValue()
        {
            foreach (var b in _singleAttackButtons)
            {
                if (b.Selected)
                {
                    return b.AttackIndex;
                }
            }
            return -1;
        }

        public override void SetValue(object obj, bool update = true)
        {
            if (update)
            {
                if (obj is int intVal)
                {
                    foreach (var b in _singleAttackButtons)
                    {
                        if (intVal == b.AttackIndex)
                        {
                            ProcessSingleAttackButton(b);
                        }
                    }
                }
            }
        }

        public override IEnumerable<Action> GetActions()
        {
            foreach (var b in _singleAttackButtons)
            {
                if (b.Selected)
                {
                    yield return Character.Actions[b.AttackIndex];
                }
            }
        }


        private void ProcessSingleAttackButton(SelectSingleAttackButton button)
        {
            if (button.Selected)
            {
                button.UnSelect();
            }
            else
            {
                foreach (var b in _singleAttackButtons)
                {
                    b.UnSelect();
                }
                button.Select();
                SetValue(new SelectAttackValue(button.AttackIndex), false);
            }
        }
        [Space]
        [SerializeField, FormerlySerializedAs("_buttonPrefab")]
        private GameObject _singleButtonPrefab;

        [SerializeField]
        private RectTransform _row;

        private List<SelectSingleAttackButton> _singleAttackButtons = new List<SelectSingleAttackButton>();
    }
}