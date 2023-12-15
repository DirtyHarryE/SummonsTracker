using SummonsTracker.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Spell
{
    public abstract class StandardSummonUndeadSpellData : SummonSpellData
    {
        public override IEnumerable<Characters.Character> GetCharacters(int spellLevel, int spellParameter, GetPerSummonParameterDelegate perSummonParameter, int number)
        {
            var num = GetNumberOfSummons(spellLevel, spellParameter, number);
            for (int i = 0; i < num; i++)
            {
                var summonParameter = perSummonParameter(i);
                yield return new Characters.Character(summonParameter == 0 ? _zombie : _skeleton);
            }
        }

        public override IEnumerable<string> GetPerSummonParameters(int spellLevel, int spellParameter)
        {
            yield return _zombie.name;
            yield return _skeleton.name;
        }

        public override int GetNumberOfSummons(int spellLevel, int spellParameter, int maxNumber)
        { 
            var extra = (spellLevel - this.Level) * ExtraSummons;
            var total = InitialNumber + extra;
            return maxNumber == -1 ? total : Mathf.Min(maxNumber, total);
        }

        protected abstract int InitialNumber { get; }
        protected abstract int ExtraSummons { get; }

        [SerializeField]
        private CharacterData _zombie;
        [SerializeField]
        private CharacterData _skeleton;
    }
}