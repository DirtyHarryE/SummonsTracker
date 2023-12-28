using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SummonsTracker.Save
{
    [System.Serializable]
    public struct SaveTarget
    {
        public static SaveTarget None = new SaveTarget("none", "None", -1, true);

        public string GUID;
        public string TargetName;
        public int ExpectedAC;
        public bool Fixed;

        public SaveTarget(string targetName)
        {
            GUID = System.Guid.NewGuid().ToString();
            TargetName = targetName;
            ExpectedAC = 0;
            Fixed = false;
        }

        public SaveTarget(string targetName, int expectedAC)
        {
            GUID = System.Guid.NewGuid().ToString();
            TargetName = targetName;
            ExpectedAC = expectedAC;
            Fixed = false;
        }

        public SaveTarget(string targetName, bool fixedAC)
        {
            GUID = System.Guid.NewGuid().ToString();
            TargetName = targetName;
            ExpectedAC = fixedAC ? -1 : 0;
            Fixed = fixedAC;
        }

        private SaveTarget(string guid, string targetName, int ac, bool fixedAC)
        {
            GUID = guid;
            TargetName = targetName;
            ExpectedAC = ac;
            Fixed = fixedAC;
        }

        public override bool Equals(object obj)
        {
            if (obj is SaveTarget target)
            {
                return GUID == target.GUID;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hc = 0;
            hc ^= GUID.GetHashCode();
            hc ^= TargetName.GetHashCode();
            hc ^= ExpectedAC.GetHashCode();
            hc ^= Fixed.GetHashCode();
            return hc;
        }

        public override string ToString() => Fixed ? TargetName : $"{TargetName}({ExpectedAC})";

        public static bool operator ==(SaveTarget a, SaveTarget b) => a.GUID == b.GUID;
        
        public static bool operator !=(SaveTarget a, SaveTarget b) => a.GUID != b.GUID;

        public static implicit operator SaveTarget (string targetName) => new SaveTarget(targetName, 10);
    }
}