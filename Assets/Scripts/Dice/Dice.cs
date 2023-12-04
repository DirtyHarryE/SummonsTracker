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
            ModifyString(dice, out int faces, out int number, out int modifiers);
            _faces = faces;
            _number = number;
            _modifiers = modifiers;
        }

        public override bool IsActive => true;
        public override int Faces => _faces;
        public override int Number => _number;
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
            ModifyString(dice, out int faces, out int number, out int modifiers);
            return new Dice( number, faces,modifiers);
        }

        public static void ModifyString(string dice, out int faces, out int number, out int modifiers)
        {
            faces = 0;
            number = 0;
            modifiers = 0;

            var dInd = dice.IndexOf('d');
            if (dInd != -1)
            {
                var facesStr = dice.Substring(0, dInd);
                var numStr = dice.Substring(dInd + 1);
                if (int.TryParse(facesStr.Trim(), out var f))
                {
                    faces = f;
                }
                if (int.TryParse(numStr.Trim(), out var n))
                {
                    number = n;
                }
            }

            var plusInd = dice.IndexOf('+');
            if (plusInd != -1)
            {
                dice = dice.Substring(0, plusInd);
                var modifiersStr = dice.Substring(plusInd + 1);
                if (int.TryParse(modifiersStr.Trim(), out var mod))
                {
                    modifiers = mod;
                }
            }
        }

        [SerializeField]
        private int _faces = 6;
        [SerializeField]
        private int _number = 1;
        [SerializeField]
        private int _modifiers = 0;

        public object Clone()
        {
            return new Dice( _number,_faces, _modifiers);
        }
    }
}