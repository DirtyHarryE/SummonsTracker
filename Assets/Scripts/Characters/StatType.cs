using System;

namespace SummonsTracker.Characters
{
    [Flags]
    public enum StatType
    {
        none = 0,
        Strength = 1,
        Dexterity = 2,
        Constitution = 4,
        Intelligence = 8,
        Wisdom = 16,
        Charisma = 32
    }
}