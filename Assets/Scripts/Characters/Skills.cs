using System;

namespace SummonsTracker.Characters
{
    [Flags]
    public enum Skills
    {
        none = 0,
        Athletics = 1,
        Acrobatics = 2,
        SleightOfHand = 4,
        Stealth = 8,
        Arcana = 16,
        History = 32,
        Investigation = 64,
        Nature = 128,
        Religion = 256,
        AnimalHandling = 512,
        Insight = 1024,
        Medicine = 2048,
        Perception = 4096,
        Survival = 8192,
        Deception = 16384,
        Intimidation = 32768,
        Performance = 65536,
        Persuasion = 131072
    }
}