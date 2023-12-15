using System;

namespace SummonsTracker.Characters
{
    [System.Serializable]
    public struct Movement
    {
        public MovementTypes Type;
        public int Distance;

        public Movement(int distance)
        {
            Type = MovementTypes.Walk;
            Distance = distance;
        }

        public Movement(MovementTypes type, int distance)
        {
            Type = type;
            Distance = distance;
        }

        public static implicit operator Movement(int distance)
        {
            return new Movement(MovementTypes.Walk, distance);
        }

        public override string ToString()
        {
            return $"{Distance} {Type}";
        }
    }
}