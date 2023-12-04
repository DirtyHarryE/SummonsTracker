using SummonsTracker.Characters;

namespace SummonsTracker.UI
{
    public class ActionReadout : Readout
    {
        public Action Action => _action;

        internal void Initialise(Action action)
        {
            _action = action;
            TitleText.text = action.Name;
            NoteText.text = action.Note;
        }

        private Action _action;
    }
}