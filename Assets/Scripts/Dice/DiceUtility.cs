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
                AdvantageType.Advantage => Mathf.Max(roll(), roll()),
                AdvantageType.Disadvantage => Mathf.Min(roll(), roll()),
                _ => roll(),
            };
            int roll()
            {
                if (number == 0 || faces == 0)
                {
                    return modifiers;
                }
                int total = 0;
                for (int i = 0; i < number; i++)
                {
                    total += Random.Range(1, faces + 1);
                }
                total += modifiers;

                return total;
            }
        }
    }
}