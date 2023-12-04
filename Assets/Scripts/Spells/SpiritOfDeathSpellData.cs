using SummonsTracker.Characters;
using SummonsTracker.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "SpiritOfDeath", menuName = "ScriptableObjects/Spells/Spirit of Death", order = 1)]
    public class SpiritOfDeathSpellData : SummonSpellData
    {
        public override IEnumerable<Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate getPerSummonParameter, int number)
        {
            var character = new Character(_reaperSpirit);

            var hpIncr = (spellLevel - 3) * 10;
            character.MaxHP += hpIncr;
            character.Hitpoints += hpIncr;

            var spellAtkMod = 10;
            var spellSaveDC = 18;
            if (GameManager.Instance != null && GameManager.Instance.StatScene != null)
            {
                spellAtkMod = GameManager.Instance.StatScene.SpellAtkMod;
                spellSaveDC = GameManager.Instance.StatScene.SpellSaveDC;
            }

            foreach (var a in character.Actions)
            {
                if (a is Multiattack multi)
                {
                    foreach (var matk in multi.Attacks)
                    {
                        matk.AttackNumber = Mathf.FloorToInt(spellLevel * 0.5f);
                    }
                }
                if (a is Attack atk)
                {
                    atk.AttackMod = spellAtkMod; 
                    foreach (var d in atk.Damages)
                    {
                        Debug.Log($"Incr: {d.DamageDice.Modifiers} + {spellLevel} = {d.DamageDice.Modifiers + spellLevel}");
                        d.DamageDice = new Rolling.Dice(d.DamageDice.Number, d.DamageDice.Faces, d.DamageDice.Modifiers + spellLevel);
                    }
                    if (atk.SavingThrow != null)
                    {
                        atk.SavingThrow = SavingThrow.Copy(atk.SavingThrow, spellSaveDC);
                    }
                }
                if (a is SavingThrowAction savingThrow)
                {
                    savingThrow.SavingThrow = SavingThrow.Copy(savingThrow.SavingThrow, spellSaveDC);
                }
            }

            yield return character;
        }

        public override int MinimumLevel => 4;

        [SerializeField]
        private CharacterData _reaperSpirit;

    }
}