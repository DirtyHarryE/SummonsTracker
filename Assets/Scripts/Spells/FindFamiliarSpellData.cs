using SummonsTracker.Characters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "Find Familiar", menuName = "ScriptableObjects/Spells/Find Familiar", order = 1)]

    public class FindFamiliarSpellData : SummonSpellData
    {
        public override IEnumerable<Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate getPerSummonParameter, int maxNumber)
        {
            var familiars = GetFamiliars();
            if (familiars != null)
            {
                var familiarData = familiars.ElementAt(parameter);
                if (familiarData != null)
                {
                    var familiar = new Character(familiarData);

                    familiar.Actions = familiar.Actions.Where(a => !(a is Attack || a is Multiattack || a is SavingThrowAction)).ToArray();

                    yield return familiar;
                }
            }
        }

        public override IEnumerable<string> GetSpellParameter(int spellLevel)
        {
            return GetFamiliars().Select(f => f.Name);
        }

        private IEnumerable<CharacterData> GetFamiliars()
        {
            foreach (var familiar in _familiars)
            {
                if (familiar != null)
                {
                    yield return familiar;
                }
            }
        }

        public override int MinimumLevel => 1;

        [SerializeField]
        private CharacterData[] _familiars;
    }
}