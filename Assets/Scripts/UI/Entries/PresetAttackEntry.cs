using SummonsTracker.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SummonsTracker.UI
{
    public class PresetAttackEntry : AttackEntry
    {
        public override void Initialise(Character character, string title, OnNewTargetMadeDelegate onNewTargetMade)
        {
            base.Initialise(character, title, onNewTargetMade);

            _attackText.text = character.Actions.First(a => !(a is Multiattack)).Name;
        }

        public override void SetValue(object obj, bool update = true) { }

        public override IEnumerable<Action> GetActions()
        {
            foreach (var action in Character.Actions)
            {
                yield return action;
            }
        }

        [SerializeField]
        private TextMeshProUGUI _attackText;
    }
}