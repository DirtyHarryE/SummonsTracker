using SummonsTracker.Characters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Spell
{
    public abstract class ConjureNormalSpellData : SummonSpellData
    {
        public override IEnumerable<Character> GetCharacters(int spellLevel, int spellParameter, GetPerSummonParameterDelegate getPerSummonParameter, int maxNumber)
        {
            var characters = GetValidSummons(spellLevel);
            var num = GetNumberOfSummons(spellLevel, spellParameter, maxNumber);
            for (int i = 0; i < num; i++)
            {
                var character = characters.ElementAt(spellParameter);
                yield return new Character(character);
            }
        }

        public override IEnumerable<string> GetSpellParameter(int spellLevel)
        {
            return GetValidSummons(spellLevel).Select(GetLabel);
        }

        protected virtual IEnumerable<CharacterData> GetValidSummons(int spellLevel)
        {
            return AllCharacters.OrderBy(c => c.Challenge).ThenBy(c => c.name);
        }

        private string GetLabel(CharacterData c)
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