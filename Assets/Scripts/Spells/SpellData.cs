using UnityEngine;

namespace SummonsTracker.Spell
{
    public abstract class SpellData : ScriptableObject
    {
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
    }
}