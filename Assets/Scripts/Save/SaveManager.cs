using System;
using System.IO;
using UnityEngine;

namespace SummonsTracker.Save
{
    public class SaveManager : Singleton<SaveManager>
    {
        public const string Filename = "Save.bin";
        public static string Path => Application.persistentDataPath;

        public string GetFullPathName()
        {
            return System.IO.Path.Combine(Path, Filename);
        }

        public SaveFile Current
        {
            get
            {
                if (_current == null)
                {
                    Load();
                }
                if (_current == null)
                {
                    _current = new SaveFile();// new WrappedSaveFile();
                    Save();
                }
                return _current;
            }
        }

        public Profile CurrentProfile
        {
            get
            {
                if (Current != null)
                {
                    if (0 <= _currentProfileIndex && _currentProfileIndex < Current.Profiles.Length)
                    {
                        return Current.Profiles[_currentProfileIndex];
                    }
                }
                return _default;
            }
        }

        public void CreateNewProfile(string newProfileName)
        {
            var newProfiles = new Profile[Current.Profiles.Length + 1];
            for (int i = 0; i < Current.Profiles.Length; i++)
            {
                newProfiles[i] = Current.Profiles[i];
            }
            newProfiles[Current.Profiles.Length] = new Profile(newProfileName);
            _currentProfileIndex = Current.Profiles.Length;
        }

        public void Load()
        {
            var internalSave = ReadFromBinaryFile(GetFullPathName());
            if (internalSave != null)
            {
                _current = internalSave;
            }
        }

        public void Save()
        {
            WriteToBinaryFile(GetFullPathName(), _current);
        }

        #region Private
        private void WriteToBinaryFile(string filePath, SaveFile save)
        {
            save.Validate();
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, save);
            }
        }

        private SaveFile ReadFromBinaryFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (SaveFile)binaryFormatter.Deserialize(stream);
            }
        }

        private SaveFile _current;

        private int _currentProfileIndex = 0;

        private Profile _default = new Profile("Summoner");
        #endregion
    }
}