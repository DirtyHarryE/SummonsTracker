using System;
namespace SummonsTracker.Characters
{
    [Flags]
    public enum ConditionTypes
    {
        none = 0,
        Blinded = 1,
        Charmed = 2,
        Deafened = 4,
        Exhaustion = 8,
        Frightened = 16,
        Grappled = 32,
        Incapacitated = 64,
        Invisible = 128,
        Paralyzed = 256,
        Petrified = 512,
        Poisoned = 1024,
        Prone = 2048,
        Restrained = 4096,
        Stunned = 8192,
        Unconscious = 16384,
    }
}