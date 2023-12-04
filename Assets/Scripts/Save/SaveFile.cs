using System;

namespace SummonsTracker.Save
{
    [Serializable]
    public class SaveFile 
    {
        public Profile[] Profiles;

        public SaveFile()
        {
            Profiles = Array.Empty<Profile>();
        }
        public SaveFile(Profile[] profiles)
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