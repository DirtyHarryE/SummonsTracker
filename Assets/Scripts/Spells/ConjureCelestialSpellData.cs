using SummonsTracker.Characters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "ConjureCelestial", menuName = "ScriptableObjects/Spells/Conjure Celestial", order = 1)]
    public class ConjureCelestialSpellData : ConjureNormalSpellData
    {
        public override int MinimumLevel => 7;

        protected override  IEnumerable<CharacterData> GetValidSummons(int spellLevel)
        {
            var targetCR = spellLevel >= 9 ? 5 : 4;
            return AllCharacters.Where(c => c.Creature == CreatureType.Celestial && c.Challenge <= targetCR)
                .OrderBy(c => c.Challenge).ThenBy(c => c.name);
        }
    }
}