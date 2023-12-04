using SummonsTracker.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Spell
{
    [CreateAssetMenu(fileName = "AnimateDead", menuName = "ScriptableObjects/Spells/Animate Dead", order = 1)]
    public class AnimateDeadSpellData : StandardSummonUndeadSpellData
    {
        public override int MinimumLevel => 3;

        protected override int InitialNumber => 2;
        protected override int ExtraSummons => 2;
    }
}