using SummonsTracker.Characters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "ConjureWoodlandBeings", menuName = "ScriptableObjects/Spells/Conjure Woodland Beings", order = 1)]
    public class ConjureWoodlandBeingsSpellData : ConjureMinorSpellData
    {
        public override int MinimumLevel => 3;

        protected override IEnumerable<CharacterData> GetValidSummons(float targetCR, int spellLevel, int spellParameter)
        {
            return AllCharacters.Where(c => c.Challenge <= targetCR && c.Creature == CreatureType.Fey)
                                .OrderBy(c => c.Challenge)
                                .ThenBy(c => c.name);
        }

        protected override string GetSpellParameterName(int spellLevel, int number, string numberString, float cr)
        {
            return $"{numberString} {(number == 1 ? "fey creature" : "fey creatures")} of CR {ChallengeRatingHelper.FloatToCR(cr).Replace("/", "\\")} or lower";
        }
    }
}