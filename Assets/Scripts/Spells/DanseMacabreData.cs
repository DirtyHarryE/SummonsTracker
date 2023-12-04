using SummonsTracker.Characters;
using SummonsTracker.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "Danse Macabre", menuName = "ScriptableObjects/Spells/Danse Macabre", order = 1)]
    public class DanseMacabreData : StandardSummonUndeadSpellData
    {
        public override IEnumerable<Characters.Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate perSummonParameter, int number)
        {
            var spellAtkMod = GameManager.Instance != null && GameManager.Instance.StatScene != null
                ? GameManager.Instance.StatScene.SpellAtkMod
                : 0;

            foreach (var c in base.GetCharacters(spellLevel, parameter, perSummonParameter, number))
            {
                foreach (var a in c.Actions)
                {
                    if (a is Attack atk)
                    {
                        atk.AttackMod += spellAtkMod;
                        foreach (var d in atk.Damages)
                        {
                            d.DamageDice = new Rolling.Dice(d.DamageDice.Number, d.DamageDice.Faces, d.DamageDice.Modifiers + spellAtkMod);
                        }
                    }
                }
                yield return c;
            }
        }

        public override int MinimumLevel => 5;

        protected override int InitialNumber => 5;
        protected override int ExtraSummons => 2;
    }
}