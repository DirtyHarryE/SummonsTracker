namespace SummonsTracker.Characters
{
    public class Action
    {
        public string Name;
        public string Note;
        public Action(ActionData actionData)
        {
            Name = actionData.name;
            Note = actionData.Note;
        }

        public Action(string name, string note)
        {
            Name = name;
            Note = note;
        }

        public override string ToString()
        {
            return $"<b>{Name}</b>. {Note}";
        }
    }
}