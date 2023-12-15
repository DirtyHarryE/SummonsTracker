using SummonsTracker.Characters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "ConjureFey", menuName = "ScriptableObjects/Spells/Conjure Fey", order = 1)]
    public class ConjureFeySpellData : ConjureNormalSpellData
    {
        public override int MinimumLevel => 6;

        protected override IEnumerable<CharacterData> GetValidSummons(int spellLevel)
        {
            var targetCR = GetTargetCR(spellLevel);
            return AllCharacters.Where(c => (c.Creature == CreatureType.Fey || c.Creature == CreatureType.Beast) && c.Challenge <= targetCR)
                .OrderBy(c => c.Challenge).ThenBy(c => c.name);
        }

        private int GetTargetCR(int spellLevel)
        {
            var above = spellLevel - MinimumLevel;
            return 6 + Mathf.Max(0, above);
        }
    }
}