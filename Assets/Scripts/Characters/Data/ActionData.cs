using UnityEngine;
using UnityEngine.Serialization;

namespace SummonsTracker.Characters
{
    [ActionMenuItem("Action")]
    public class ActionData : ScriptableObject
    {
        public string Note => _note;

        [SerializeField, TextArea]
        private string _note;

        public virtual Action Instantiate()
        {
            return new Action(this);
        }
    }
}