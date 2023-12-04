namespace SummonsTracker.Characters
{
    public static class SkillsUtility
    {
        public static StatType GetStatType(Skills skills)
        {
            var statType = StatType.none;

            if ((skills & Skills.Athletics) != 0)
            {
                statType |= StatType.Strength;
            }
            if ((skills & (Skills.Acrobatics | Skills.SleightOfHand | Skills.Stealth)) != 0)
            {
                statType |= StatType.Dexterity;
            }
            if ((skills & (Skills.Arcana | Skills.History | Skills.Investigation | Skills.Nature | Skills.Religion)) != 0)
            {
                statType |= StatType.Intelligence;
            }
            if ((skills & (Skills.AnimalHandling | Skills.Insight | Skills.Medicine | Skills.Perception | Skills.Survival)) != 0)
            {
                statType |= StatType.Wisdom;
            }
            if ((skills & (Skills.Deception | Skills.Intimidation | Skills.Performance | Skills.Persuasion)) != 0)
            {
                statType |= StatType.Charisma;
            }
            return statType;
        }

        private static void checkSkills(Skills skills, Skills skill, ref StatType currentStats)
        {
            if ((skill & skills) != 0)
            {
                currentStats |= StatType.Strength;
            }
        }
    }
}