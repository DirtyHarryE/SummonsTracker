using SummonsTracker.Licensing;
using SummonsTracker.Loading;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Spell
{
    public abstract class SpellData : LicensedScriptableObject, ILoadable<SpellData>
    {
        public static IReadOnlyList<SpellData> AllSpells => _allSpells;
        private static List<SpellData> _allSpells = new List<SpellData>();

        public bool Concentration => _concentration;
        public int Level => _level;

        public abstract int MinimumLevel { get; }

        protected virtual void OnValidate()
        {
            _level = Mathf.Max(_level, MinimumLevel);
        }

        protected virtual void Reset()
        {
            _level = MinimumLevel;
        }

        [SerializeField]
        private bool _concentration;
        [SerializeField]
        private int _level;

        void ILoadable.Load()
        {
            _allSpells.Add(this);
        }

        int ILoadable.Priority => 1;
    }
}