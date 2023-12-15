using SummonsTracker.Characters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "CreateUndead", menuName = "ScriptableObjects/Spells/Create Undead", order = 1)]
    public class CreateUndeadSpellData : SummonSpellData
    {
        public override IEnumerable<Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate getPerSummonParameter, int maxNumber)
        {
            var summons = GetSummons(spellLevel);
            var n = GetNumberOfSummons(spellLevel, parameter, maxNumber);
            return GetCharacters(summons.FirstOrDefault(s => s.Item1 == spellLevel).Item3, n);
        }

        public override IEnumerable<string> GetSpellParameter(int spellLevel)
        {
            return GetSummons(spellLevel).Select(s => $"{s.Item2} {s.Item3.name}s");
        }

        public override int MinimumLevel => 6;

        public override int GetNumberOfSummons(int spellLevel, int parameter, int maxNumber)
        {
            var parameters = GetSummons(spellLevel).ToArray();
            var p = parameters[Mathf.Clamp(parameter, 0, parameters.Length - 1)];

            var number = p.Item2;

            return maxNumber == -1 ? number : Mathf.Min(maxNumber, number);
        }

        private IEnumerable<Character> GetCharacters(CharacterData characterData, int number)
        {
            if (characterData != null)
            {
                for (int i = 0; i < number; i++)
                {
                    yield return new Character(characterData);
                }
            }
        }

        private IEnumerable<(int, int, CharacterData)> GetSummons(int spellLevel)
        {
            switch (spellLevel)
            {
                case 6:
                    yield return (6, 3, _ghoul);
                    break;
                case 7:
                    yield return (7, 4, _ghoul);
                    break;
                case 8:
                    yield return (8, 5, _ghoul);
                    yield return (8, 2, _ghast);
                    yield return (8, 2, _wight);
                    break;
                case 9:
                    yield return (9, 6, _ghoul);
                    yield return (9, 3, _ghast);
                    yield return (9, 3, _wight);
                    yield return (9, 2, _mummy);
                    break;
            }
        }
        [SerializeField]
        private CharacterData _ghoul;
        [SerializeField]
        private CharacterData _ghast;
        [SerializeField]
        private CharacterData _wight;
        [SerializeField]
        private CharacterData _mummy;
    }
}