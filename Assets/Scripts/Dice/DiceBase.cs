using UnityEngine;

namespace SummonsTracker.Rolling
{
    public enum AdvantageType
    {
        None,
        Advantage,
        Disadvantage
    }
    [System.Serializable]
    public abstract class DiceBase
    {
        public abstract bool IsActive { get; }
        public abstract int Faces { get; }
        public abstract int Number { get; }
        public abstract int Modifiers { get; }

        public virtual int Roll(AdvantageType advantageType = AdvantageType.None) => DiceUtility.Roll(this, advantageType);

        public virtual int Average => Mathf.FloorToInt(((Faces + 1) * 0.5f * Number) + Modifiers);

        public override string ToString()
        {
            return Faces == 0 || Number == 0 ? Modifiers.ToString()
                : Modifiers > 0 ? $"{Average} ({Number}d{Faces}+{Modifiers})"
                : Modifiers < 0 ? $"{Average} ({Number}d{Faces}{Modifiers})"
                : $"{Average} ({Number}d{Faces})";
        }
        public static bool operator ==(DiceBase dice1, DiceBase dice2)
        {
            return dice1.IsActive == dice2.IsActive
                && dice1.Faces == dice2.Faces
                && dice1.Number == dice2.Number
                && dice1.Modifiers == dice2.Modifiers;
        }
        public static bool operator !=(DiceBase dice1, DiceBase dice2) => !(dice1 == dice2);
        public override bool Equals(object obj) => obj is DiceBase otherDice && this == otherDice;
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IsActive.GetHashCode();
                hashCode = (hashCode * 397) ^ Faces.GetHashCode();
                hashCode = (hashCode * 397) ^ Number.GetHashCode();
                hashCode = (hashCode * 397) ^ Modifiers.GetHashCode();
                return hashCode;
            }
        }
    }
}