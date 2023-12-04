using System;
using System.Collections.Generic;
using System.Linq;

namespace SummonsTracker.Spell
{
    public abstract class SummonSpellData : SpellData
    {
        public delegate int GetPerSummonParameterDelegate(int index);

        public virtual IEnumerable<Characters.Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate getPerSummonParameter)
            => GetCharacters(spellLevel, parameter, getPerSummonParameter, -1);
        public abstract IEnumerable<Characters.Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate getPerSummonParameter, int maxNumber);

        public virtual IEnumerable<string> GetSpellParameter(int spellLevel)
        {
            return Enumerable.Empty<string>();
        }
        public virtual IEnumerable<string> GetPerSummonParameters(int spellLevel)
        {
            return Enumerable.Empty<string>();
        }
        public virtual int GetNumberOfSummons(int spellLevel, int parameter) => GetNumberOfSummons(spellLevel, parameter, -1);
        public virtual int GetNumberOfSummons(int spellLevel, int parameter, int maxNumber) => 1;
    }
}