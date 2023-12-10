using SummonsTracker.Characters;
using System;

namespace SummonsTracker.Save
{
    [Serializable]
    public class SaveCharacter
    {
        public string DataName;
        public string Name;
        public int HP;
        public int MaxHP;
        public int TempHP;
        public ConditionTypes Condition;
        public bool Concentration;

        public SaveCharacter(Character character, bool concentration) : this(dataName: character.CharacterData.name,
                                                                             name: character.Name,
                                                                             hP: character.Hitpoints,
                                                                             maxHP: character.MaxHP,
                                                                             tempHP: character.TemporaryHitPoints,
                                                                             condition: character.Conditions,
                                                                             concentration: concentration)
        {

        }

        public SaveCharacter(string dataName, string name, int hP, int maxHP, int tempHP, ConditionTypes condition, bool concentration)
        {
            DataName = dataName;
            Name = name;
            HP = hP;
            MaxHP = maxHP;
            TempHP = tempHP;
            Condition = condition;
            Concentration = concentration;
        }

        public void Validate()
        {
            
        }
    }
}