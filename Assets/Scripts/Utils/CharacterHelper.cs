using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public static class CharacterHelper
    {
        public static int GetMod(int score) => Mathf.FloorToInt((score - 10) * 0.5f);
    }
}