using System;

namespace SummonsTracker.Save
{
    [Serializable]
    public class SaveFile 
    {
        public Profile[] Profiles;
        public int CurrentProfileIndex = 0;

        public SaveFile(params Profile[] profiles)
        {
            Profiles = profiles;
        }

        public void Validate()
        {
            foreach (var p in Profiles)
            {
                p.Validate();
            }
        }
    }
}