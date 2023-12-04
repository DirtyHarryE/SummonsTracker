
using System;

namespace SummonsTracker.Characters
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ActionMenuItem : Attribute
    {
        private string menuPath;
        public ActionMenuItem(string menuPath)
        {
            this.menuPath = menuPath;
        }
        public string MenuPath
        {
            get
            {
                return menuPath;
            }
        }
    }
}