using UnityEngine;

namespace SummonsTracker.Rolling
{
    public static class DiceUtility
    {
        public static int Roll(DiceBase dice, AdvantageType advantageType = AdvantageType.None)
            => Roll(dice.Number, dice.Faces, dice.Modifiers, advantageType);

        public static int Roll(int number, int faces, int modifiers, AdvantageType advantageType = AdvantageType.None)
        {
            return advantageType switch
            {
                AdvantageType.Advantage
                    => Mathf.Max(RollInternal(number, faces, modifiers), RollInternal(number, faces, modifiers)),
                AdvantageType.Disadvantage
                    => Mathf.Min(RollInternal(number, faces, modifiers), RollInternal(number, faces, modifiers)),
                _ => RollInternal(number, faces, modifiers),
            };
        }

        public static int Average(this DiceBase diceBase)
        {
            return Mathf.FloorToInt(((diceBase.Faces + 1) * 0.5f * diceBase.Number) + diceBase.Modifiers);
        }

        public static int Average(int number, int faces, int modifiers)
        {
            return Mathf.FloorToInt(((faces + 1) * 0.5f * number) + modifiers);
        }

        public static void FromString(string dice, out int number, out int faces, out int modifiers)
        {
            var bOpenInd = dice.IndexOf('(');
            var bCloseInd = dice.IndexOf(')');

            if (bOpenInd != -1 && bCloseInd != -1)
            {
                dice = dice.Substring(bOpenInd + 1, bCloseInd - bOpenInd - 1);
            }

            number = 0;
            faces = 0;
            modifiers = 0;

            var dInd = dice.IndexOf('d');
            var plusInd = dice.IndexOf('+');
            if (dInd != -1)
            {
                var numStr = dice.Substring(0, dInd);
                var facesStr = plusInd == -1 
                    ? dice.Substring(dInd + 1)
                    : dice.Substring(dInd + 1, plusInd - dInd - 1);
                if (int.TryParse(numStr.Trim(), out var n))
                {
                    number = n;
                }
                if (int.TryParse(facesStr.Trim(), out var f))
                {
                    faces = f;
                }
            }

            if (plusInd != -1)
            {
                var modifiersStr = dice.Substring(plusInd + 1);
                if (int.TryParse(modifiersStr.Trim(), out var mod))
                {
                    modifiers = mod;
                }
            }
        }

        private static int RollInternal(int number, int faces, int modifiers)
        {
            if (number == 0 || faces == 0)
            {
                return modifiers;
            }
            int total = 0;
            for (int i = 0; i < number; i++)
            {
                var r = _random.Next(1, faces + 1);
                total += r;
            }
            total += modifiers;
            return total;
        }

        private static System.Random _random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
    }
}