using System;

namespace SummonsTracker.Save
{
    [System.Serializable]
    public class Profile
    {
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

        public Profile(string name, params SaveCharacter[] saveCharacters)
        {
            Name = name;
            SaveCharacters = saveCharacters;
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