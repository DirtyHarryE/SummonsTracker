using SummonsTracker.Characters;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "CreateUndead", menuName = "ScriptableObjects/Spells/Create Undead", order = 1)]
    public class CreateUndeadSpellData : SummonSpellData
    {
        public override IEnumerable<Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate getPerSummonParameter, int maxNumber)
        {
            var n = GetNumberOfSummons(spellLevel, parameter, maxNumber);
            switch (spellLevel)
            {
                case 6:
                    return GetCharacters(_ghoul, n);
                case 7:
                    return GetCharacters(_ghoul, n);
                case 8:
                    switch (parameter)
                    {
                        case 0: return GetCharacters(_ghoul, n);
                        case 1: return GetCharacters(_ghast, n);
                        case 2: return GetCharacters(_wight, n);
                    }
                    break;
                case 9:
                    switch (parameter)
                    {
                        case 0: return GetCharacters(_ghoul, n);
                        case 1: return GetCharacters(_ghast, n);
                        case 2: return GetCharacters(_wight, n);
                        case 3: return GetCharacters(_mummy, n);
                    }
                    break;
            }

            return Enumerable.Empty<Character>();
        }

        public override IEnumerable<string> GetSpellParameter(int spellLevel)
        {
            switch (spellLevel)
            {
                case 6:
                    yield return "3 Ghouls";
                    break;
                case 7:
                    yield return "4 Ghouls";
                    break;
                case 8:
                    yield return "5 Ghouls";
                    yield return "2 Ghasts";
                    yield return "2 Wights";
                    break;
                case 9:
                    yield return "6 Ghouls";
                    yield return "3 Ghasts";
                    yield return "3 Wights";
                    yield return "2 Mummies";
                    break;
            }
        }

        public override int MinimumLevel => 6;

        public override int GetNumberOfSummons(int spellLevel, int parameter, int maxNumber)
        {
            var parameters = GetSpellParameter(spellLevel).ToArray();
            var p = parameters[Mathf.Clamp(parameter, 0, parameters.Length - 1)];

            var pNum = Regex.Match(p, @"\d+").Value;

            if (int.TryParse(pNum, out int number))
            {
                return maxNumber == -1 ? number : Mathf.Min(maxNumber, number);
            }
            return base.GetNumberOfSummons(spellLevel, parameter, maxNumber);
        }

        private IEnumerable<Character> GetCharacters(CharacterData characterData, int number)
        {
            for (int i = 0; i < number; i++)
            {
                yield return new Character(characterData);
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