using System;

namespace SummonsTracker.Save
{
    [System.Serializable]
    public class Profile
    {
        private static SaveTarget[] DefaultTargets => new SaveTarget[] { "Target One", "Target Two" };

        public string Name;

        public int WizardLevel = 12;
        public int SpellcastingAbility = 5;
        public int SpellAtkMod = 10;
        public int SpellSaveDC = 18;
        public int Proficiency = 4;
        public int HpMax = 86;
        public int Strength = 6;
        public int Dexterity = 14;
        public int Constitution = 17;
        public int Intelligence = 20;
        public int Wisdom = 11;
        public int Charisma = 12;

        public SaveCharacter[] SaveCharacters;

        public SaveTarget[] SavedTargets = DefaultTargets;

        public Profile(string name)
        {
            Name = name;
            SavedTargets = DefaultTargets;
            SaveCharacters = Array.Empty<SaveCharacter>();
        }

        public Profile(string name, SaveCharacter[] saveCharacters)
        {
            Name = name;
            SavedTargets = DefaultTargets;
            SaveCharacters = saveCharacters;
        }

        public Profile(string name, SaveCharacter[] saveCharacters, SaveTarget[] targets)
        {
            Name = name;
            SaveCharacters = saveCharacters;
            SavedTargets = targets;
        }

        public void Validate()
        {
            foreach (var s in SaveCharacters)
            {
                s.Validate();
            }
        }
    }
}