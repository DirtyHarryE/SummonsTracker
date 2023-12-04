using System;

namespace SummonsTracker.Save
{
    [System.Serializable]
    public class Profile
    {
        public string Name;
        public SaveCharacter[] SaveCharacters;


        public Profile() : this("Summoner", Array.Empty<SaveCharacter>()) { }
        public Profile(string name) : this(name, Array.Empty<SaveCharacter>()) { }
        public Profile(SaveCharacter[] saveCharacters) : this("Summoner", saveCharacters) { }
        public Profile(string name, SaveCharacter[] saveCharacters)
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