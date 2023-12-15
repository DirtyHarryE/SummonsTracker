using SummonsTracker.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "ConjureElemental", menuName = "ScriptableObjects/Spells/Conjure Elemental", order = 1)]
    public class ConjureElementalSpellData : SummonSpellData
    {
        public enum ElementalTypes
        {
            Air,
            Earth,
            Fire,
            Water,
            All
        }

        [System.Serializable]
        public struct Elementals
        {
            public ElementalTypes ElementalType;
            public List<CharacterData> Characters;

            public Elementals(ElementalTypes elementalType, List<CharacterData> characters)
            {
                ElementalType = elementalType;
                Characters = characters;
            }
        }

        public override int MinimumLevel => 5;

        public override IEnumerable<Character> GetCharacters(int spellLevel, int parameter, GetPerSummonParameterDelegate getPerSummonParameter, int maxNumber)
        {
            for (int i = 0; i < GetNumberOfSummons(spellLevel, parameter); i++)
            {
                var data = GetValidSummons(spellLevel, parameter).ElementAt(getPerSummonParameter(i));
                yield return new Character(data);
            }
        }

        public override IEnumerable<string> GetSpellParameter(int spellLevel)
        {
            foreach (var element in Enum.GetNames(typeof(ElementalTypes)))
            {
                yield return element;
            }
        }

        public override IEnumerable<string> GetPerSummonParameters(int spellLevel, int spellParameter)
        {
            return GetValidSummons(spellLevel, spellParameter).Select(c => c.Name);
        }

        private IEnumerable<CharacterData> GetValidSummons(int spellLevel, int spellParameter)
        {
            var elementType = (ElementalTypes)spellParameter;
            var targetCR = GetTargetCR(spellLevel);

            var hardCoded = _hardCodedElementals.FirstOrDefault(e => e.ElementalType == elementType).Characters;

            var list = new List<CharacterData>();
            foreach (var elist in _hardCodedElementals)
            {
                if (elementType!= ElementalTypes.All && elist.ElementalType == elementType)
                {
                    foreach (var elemental in elist.Characters)
                    {
                        if (IsCreatureValid(elemental, spellLevel, spellParameter, elementType, targetCR, false))
                        {
                            list.Add(elemental);
                        }
                    }
                }
            }
            list.AddRange(AllCharacters.Where(c => IsCreatureValid(c, spellLevel, spellParameter, elementType, targetCR, elementType != ElementalTypes.All)));
            return list.Distinct()
                       .OrderBy(c => c.Challenge)
                       .ThenBy(c => c.name);
        }

        private bool IsCreatureValid(CharacterData characterData, int spellLevel, int spellParameter, ElementalTypes elementType, float targetCR, bool checkName)
        {
            if (characterData == null)
            {
                return false;
            }
            if (characterData.Creature != CreatureType.Elemental)
            {
                return false;
            }
            if (characterData.Challenge > targetCR)
            {
                return false;
            }
            if (checkName && !characterData.Name.ToLower().Contains(elementType.ToString().ToLower()))
            {
                return false;
            }
            return true;
        }

        private int GetTargetCR(int spellLevel)
        {
            var above = spellLevel - MinimumLevel;
            return 5 + Mathf.Max(0, above);
        }


        public Dictionary<ElementalTypes, IEnumerable<CharacterData>> _myDictionary = new Dictionary<ElementalTypes, IEnumerable<CharacterData>>();

        [SerializeField]
        private List< Elementals> _hardCodedElementals;
    }
}