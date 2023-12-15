using SummonsTracker.Characters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "ConjureMinorElementals", menuName = "ScriptableObjects/Spells/Conjure Minor Elementals", order = 1)]
    public class ConjureMinorElementalsSpellData : ConjureMinorSpellData
    {
        public override int MinimumLevel => 4;

        protected override IEnumerable<CharacterData> GetValidSummons( float targetCR, int spellLevel, int spellParameter)
        {
            return AllCharacters.Where(c => c.Challenge <= targetCR && c.Creature == CreatureType.Elemental)
                                .OrderBy(c => c.Challenge)
                                .ThenBy(c => c.name);
        }

        protected override string GetSpellParameterName(int spellLevel, int number, string numberString, float cr)
        {
            return $"{numberString} {(number == 1 ? "elemental" : "elementals")} of CR {ChallengeRatingHelper.FloatToCR(cr).Replace("/", "\\")} or lower";
        }
    }
}