using System.IO;
using System.Text;
using UnityEngine;

namespace SummonsTracker.Save
{
    public class SaveManager : Singleton<SaveManager>
    {
        public const string Filename =
            #if PLATFORM_ANDROID
             "Saves/saveData.txt";
#else
            "Save.bin";
#endif
        public static string Path => Application.persistentDataPath;

        public string GetFullPathName()
        {
#if PLATFORM_ANDROID
            return $"{Application.persistentDataPath}/{Filename}";
#else
            return System.IO.Path.Combine(Path, Filename);
#endif
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
                    _current = new SaveFile(new Profile("Summoner"));// new WrappedSaveFile();
                    Save();
                }
                return _current;
            }
        }

        public Profile LoadProfile(int index)
        {
            if (0 <= index && index < Current.Profiles.Length)
            {
                Current.CurrentProfileIndex = index;
                return Current.Profiles[index];
            }
            return null;
        }

        public Profile CurrentProfile
        {
            get
            {
                if (Current != null)
                {
                    if (0 <= Current.CurrentProfileIndex && Current.CurrentProfileIndex < Current.Profiles.Length)
                    {
                        return Current.Profiles[Current.CurrentProfileIndex];
                    }
                }
                return _default;
            }
        }

        public Profile CreateNewProfile(string newProfileName)
        {
            var newProfile = new Profile(newProfileName);
            int l = Current.Profiles.Length;
            var newProfiles = new Profile[l + 1];
            for (int i = 0; i < l; i++)
            {
                newProfiles[i] = Current.Profiles[i];
            }
            newProfiles[l] = newProfile;
            Current.CurrentProfileIndex = l;
            Current.Profiles = newProfiles;
            return newProfile;
        }

        public bool DeleteProfile(int index)
        {
            var newProfiles = new Profile[Current.Profiles.Length - 1];
            for (int i = 0; i < Current.Profiles.Length - 1; i++)
            {
                newProfiles[i] = Current.Profiles[i >= index ? i + 1 : i];
            }
            Current.Profiles = newProfiles;
            return true;
        }

        public void Load()
        {
            var pathname = GetFullPathName();
            try
            {
                var dir = System.IO.Path.GetDirectoryName(pathname);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var internalSave = ReadFromBinaryFile(pathname);
                if (internalSave != null)
                {
                    _current = internalSave;


                    var builder = new StringBuilder();
                    builder.Append(pathname).AppendLine().Append("Save: ").Append(internalSave != null);
                    for (int i = 0; i < _current.Profiles.Length; i++)
                    {
                        builder.AppendLine().Append(_current.Profiles[i].Name);
                        for (int j = 0; j < _current.Profiles[i].SaveCharacters.Length; j++)
                        {
                            builder.Append(" - ").Append(_current.Profiles[i].SaveCharacters[j].Name).Append("(").Append(_current.Profiles[i].SaveCharacters[j].DataName).Append(")");
                        }
                    }
                    Debug.Log(builder.ToString());
                }
                else
                {
                    Debug.LogError($"Did not load\n{pathname}");
                }
            } catch (System.Exception e)
            {
                Debug.LogError($"Cannot Load: \"{pathname}\"\n{e}");
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
            using Stream stream = File.Open(filePath, FileMode.Create);
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, save);
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

        private Profile _default = new Profile("Summoner");
#endregion
    }
}