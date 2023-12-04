using System;

namespace SummonsTracker.Characters
{
    [Flags]
    public enum SavingThrowType
    {
        none = 0,
        Strength = 1,
        Dexterity = 2,
        Constitution = 4,
        Intelligence = 8,
        Wisdom = 16
    }
}