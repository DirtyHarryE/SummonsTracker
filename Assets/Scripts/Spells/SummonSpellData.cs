using SummonsTracker.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public virtual IEnumerable<string> GetPerSummonParameters(int spellLevel, int spellParameter)
        {
            return Enumerable.Empty<string>();
        }
        public virtual int GetNumberOfSummons(int spellLevel, int parameter) => GetNumberOfSummons(spellLevel, parameter, -1);
        public virtual int GetNumberOfSummons(int spellLevel, int parameter, int maxNumber) => 1;

        protected static IEnumerable<CharacterData> AllCharacters
        {
            get
            {
                if (_allCharacters == null || !_allCharacters.Any())
                {
                    _allCharacters = LoadAllCharacters();
                }
                return _allCharacters;
            }
        }

        private static IEnumerable<CharacterData> LoadAllCharacters()
        {
#if UNITY_EDITOR
            return Application.isPlaying
                ? CharacterData.AllCharacters
                : UnityEditor.AssetDatabase.FindAssets("t:CharacterData")
                                           .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                                           .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<CharacterData>);
#else
            return CharacterData.AllCharacters;
#endif
        }

        private static IEnumerable<CharacterData> _allCharacters;
    }
}