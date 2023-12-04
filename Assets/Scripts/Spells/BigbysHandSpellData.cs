using SummonsTracker.Characters;
using SummonsTracker.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "BigbysHand", menuName = "ScriptableObjects/Spells/Bigby's Hand", order = 1)]
    public class BigbysHandSpellData : SummonSpellData
    {
        public override IEnumerable<Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate getPerSummonParameter, int number)
        {
            var character = new Character(_bigbysHand);

            var diff = spellLevel - Level;

            var spellAttackMod = GameManager.Instance != null && GameManager.Instance.StatScene != null
                ? GameManager.Instance.StatScene.SpellAtkMod
                : 0;
            var hpMax = GameManager.Instance != null && GameManager.Instance.StatScene != null
                ? GameManager.Instance.StatScene.HpMax
                : 0;

            character.Hitpoints = hpMax;
            character.MaxHP = hpMax;

            foreach (var a in character.Actions)
            {
                if (a is Attack atk)
                {
                    atk.AttackMod = spellAttackMod;
                    foreach (var d in atk.Damages)
                    {
                        d.DamageDice = new Rolling.Dice(number: d.DamageDice.Number + (2 * diff),
                                                        faces: d.DamageDice.Faces,
                                                        modifiers: a.Name.Contains("Grasping") ? spellAttackMod : d.DamageDice.Modifiers);
                    }
                }
            }
            yield return character;
        }

        public override int MinimumLevel => 5;

        [SerializeField]
        private CharacterData _bigbysHand;
    }
}