using System;
using UnityEngine;

namespace SummonsTracker.Rolling
{
    [Serializable]
    public class Dice : DiceBase, ICloneable
    {
        public static Dice None => new Dice(0, 0, 0);
        public static Dice D20 => new Dice(1, 20, 0);
        public static Dice D6 => new Dice(1, 6, 0);

        public Dice(int number, int faces, int modifiers)
        {
            _number = number;
            _faces = faces;
            _modifiers = modifiers;
        }

        public Dice(string dice)
        {
            DiceUtility.FromString(dice, out int number, out int faces, out int modifiers);
            _faces = faces;
            _number = number;
            _modifiers = modifiers;
        }

        public override bool IsActive => true;
        public override int Number => _number;
        public override int Faces => _faces;
        public override int Modifiers => _modifiers;

        public override string ToString()
        {
            return Faces == 0 || Number == 0 ? Modifiers.ToString()
                : Modifiers > 0 ? $"{Average} ({Number}d{Faces}+{Modifiers})"
                : Modifiers < 0 ? $"{Average} ({Number}d{Faces}{Modifiers})"
                : $"{Average} ({Number}d{Faces})";
        }

        public static implicit operator Dice(string dice)
        {
            DiceUtility.FromString(dice, out int number, out int faces, out int modifiers);
            return new Dice(number, faces, modifiers);
        }

        [SerializeField]
        private int _number = 1;
        [SerializeField]
        private int _faces = 6;
        [SerializeField]
        private int _modifiers = 0;

        public object Clone()
        {
            return new Dice(_number, _faces, _modifiers);
        }
    }
}