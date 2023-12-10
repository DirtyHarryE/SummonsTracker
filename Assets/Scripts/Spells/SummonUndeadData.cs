using SummonsTracker.Characters;
using SummonsTracker.Manager;
using SummonsTracker.Save;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "SummonUndead", menuName = "ScriptableObjects/Spells/Summon Undead", order = 1)]
    public class SummonUndeadData : SummonSpellData
    {
        public override IEnumerable<Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate getPerSummonParameter, int number)
        {
            var data = GetCharacter(parameter);
            var character = new Character(data);

            var diff = spellLevel - 3;

            character.MaxHP += diff * 10;

            var spellAtkMod = 10;
            var spellSaveDC = 18;
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentProfile != null)
            {
                spellAtkMod = SaveManager.Instance.CurrentProfile.SpellAtkMod;
                spellSaveDC = SaveManager.Instance.CurrentProfile.SpellSaveDC;
            }

            foreach (var a in character.Actions)
            {
                if (a is Attack atk)
                {
                    atk.AttackMod = spellAtkMod;
                    foreach (var d in atk.Damages)
                    {
                        d.DamageDice = new Rolling.Dice(d.DamageDice.Number, d.DamageDice.Faces, d.DamageDice.Modifiers + spellLevel);
                    }
                    if (atk.SavingThrow != null)
                    {
                        atk.SavingThrow = SavingThrow.Copy(atk.SavingThrow, spellSaveDC);
                    }
                }
                if (a is SavingThrowAction savingThrowAction)
                {
                    savingThrowAction.SavingThrow = SavingThrow.Copy(savingThrowAction.SavingThrow, spellSaveDC);
                }
                if (a is Multiattack multi)
                {
                    foreach (var matk in multi.Attacks)
                    {
                        matk.AttackNumber = Mathf.FloorToInt(spellLevel * 0.5f);
                    }
                }
            }

            yield return character;
        }

        public override IEnumerable<string> GetSpellParameter(int spellLevel)
        {
            yield return "Skeletal";
            yield return "Ghostly";
            yield return "Putrid";
        }

        public override int MinimumLevel => 3;

        private CharacterData GetCharacter(int parameter) => parameter switch
        {
            0 => _skeletal,
            1 => _ghostly,
            2 => _putrid,
            _ => null,
        };

        [SerializeField]
        private CharacterData _skeletal;
        [SerializeField]
        private CharacterData _ghostly;
        [SerializeField]
        private CharacterData _putrid;

    }
}