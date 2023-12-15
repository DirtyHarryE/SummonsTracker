using SummonsTracker.Characters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Spell
{
    public abstract class ConjureMinorSpellData : SummonSpellData
    {
        protected struct NumAndCR
        {
            public int Number;
            public float CR;

            public NumAndCR(int number, float cR)
            {
                Number = number;
                CR = cR;
            }
            public static implicit operator NumAndCR((int, float) t) => new NumAndCR(t.Item1, t.Item2);
        }
        public override IEnumerable<Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate getPerSummonParameter, int maxNumber)
        {
            var characters = GetValidSummons(spellLevel, parameter);
            var num = GetNumberOfSummons(spellLevel, parameter, maxNumber);
            for (int i = 0; i < num; i++)
            {
                var summonParameter = getPerSummonParameter(i);
                var character = characters.ElementAt(summonParameter);
                yield return new Characters.Character(character);
            }
        }

        public override int GetNumberOfSummons(int spellLevel, int parameter, int maxNumber)
        {
            var num = GetCR().ElementAt(parameter).Number;

            switch (spellLevel)
            {
                case 5:
                case 6:
                    num *= 2;
                    break;
                case 7:
                case 8:
                    num *= 3;
                    break;
                case 9:
                    num *= 4;
                    break;
            }
            return maxNumber != -1 ? Mathf.Min(maxNumber, num) : num;
        }

        public override IEnumerable<string> GetPerSummonParameters(int spellLevel, int spellParameter)
        {
            return GetValidSummons(spellLevel, spellParameter).Select(GetLabel);
        }

        public override IEnumerable<string> GetSpellParameter(int spellLevel)
        {
            var index = 0;
            foreach (var cr in GetCR())
            {
                yield return GetSpellParameterName(spellLevel, cr.Number, GetNumber(GetNumberOfSummons(spellLevel, index++, -1)), cr.CR);
            }
        }

        protected virtual IEnumerable<CharacterData> GetValidSummons(float targetCR, int spellLevel, int spellParameter)
        {
            return AllCharacters.Where(c => c.Challenge <= targetCR)
                                .OrderBy(c => c.Challenge)
                                .ThenBy(c => c.name);
        }

        protected virtual string GetSpellParameterName(int spellLevel, int number, string numberString, float cr)
        {
            return $"{number} {(number == 1 ? "creature" : "creatures")} of CR {ChallengeRatingHelper.FloatToCR(cr).Replace("/", "\\")} or lower";
        }

        protected virtual IEnumerable<NumAndCR> GetCR()
        {
            yield return (1, 2);
            yield return (2, 1);
            yield return (4, 0.5f);
            yield return (8, 0.25f);
        }

        private string GetNumber(int number) => number switch
        {
            0 => "Zero",
            1 => "One",
            2 => "Two",
            3 => "Three",
            4 => "Four",
            5 => "Five",
            6 => "Six",
            7 => "Seven",
            8 => "Eight",
            9 => "Nine",
            10 => "Ten",
            _ => number.ToString(),
        };

        private IEnumerable<CharacterData> GetValidSummons(int spellLevel, int spellParameter)
        {
            var targetCR = GetCR().ElementAt(spellParameter).CR;
            return GetValidSummons(targetCR, spellLevel, spellParameter);
        }

        protected virtual string GetLabel(CharacterData c)
        {
            var s = c.Name;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                s = $"CR {ChallengeRatingHelper.FloatToCR(c.Challenge).Replace("/", "\u2044")}/{s}";
            }
#endif
            return s;
        }
    }
}