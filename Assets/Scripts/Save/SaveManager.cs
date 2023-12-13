using SummonsTracker.Characters;
using SummonsTracker.Rolling;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SummonsTracker.Save
{
    public class SaveManager : Singleton<SaveManager>
    {
        public const string Filename =
#if PLATFORM_ANDROID && !UNITY_EDITOR
             "Saves/saveData.txt";
#else
            "Save.bin";
#endif
        public const string SaveAttackName = "Attack";
        public const string SaveSavingThrowName = "SavingThrow";
        public const string SaveMultiattackName = "Multiattack";
        public const string SaveActionName = "Action";
        public static string Path => Application.persistentDataPath;

        public static SaveAction Convert(Action action)
        {
            if (action is Attack attack)
            {
                return new SaveAction(SaveAttackName, attack.Name, attack.Note, attack.AttackType, attack.AttackMod, attack.AdvantageType, attack.Range,
                    attack.MaxRange, attack.Target, attack.Damages.Select(Convert).ToArray(), Convert(attack.SavingThrow));
            }
            else if (action is SavingThrowAction savingThrow)
            {
                return new SaveAction(SaveSavingThrowName, savingThrow.Name, savingThrow.Note, Convert(savingThrow: savingThrow));
            }
            else if (action is Multiattack multiattack)
            {
                return new SaveAction(SaveMultiattackName, multiattack.Name, multiattack.Note, multiattack.Attacks.Select(Convert).ToArray());
            }
            else
            {
                return new SaveAction(SaveActionName, action.Name, action.Note);
            }
        }

        public static SaveDamage Convert(Attack.Damage damage)
        {
            return new SaveDamage(damage.DamageDice.Number, damage.DamageDice.Faces, damage.DamageDice.Modifiers, damage.DamageType);
        }

        public static SaveDamage Convert(DiceBase dice, DamageTypes damageType)
        {
            return new SaveDamage(dice.Number, dice.Faces, dice.Modifiers, damageType);
        }

        public static SaveSavingThrow Convert(ISavingThrow savingThrow)
        {
            return new SaveSavingThrow(savingThrow.IsGrapple,
                savingThrow.SavingThrow,
                savingThrow.DC,
                Convert(savingThrow.FailureSavingThrowOutcome),
                Convert(savingThrow.SuccessSavingThrowOutcome));
        }

        public static SaveThrowOutcome Convert(SuccessSaveOutcome success)
        {
            return new SaveThrowOutcome(success.SuccessSaveType.ToString(), Convert(success.Damage, success.DamageType), success.Condition, success.OutcomeNote);
        }

        public static SaveThrowOutcome Convert(FailSaveOutcome fail)
        {
            return new SaveThrowOutcome(fail.FailSaveType.ToString(), Convert(fail.Damage, fail.DamageType), fail.Condition, fail.OutcomeNote);
        }

        public static SaveMultiAttack Convert(Multiattack.MultiattackInfo multiattack)
        {
            return new SaveMultiAttack(multiattack.AnyAttack ? -1 : multiattack.AttackIndex, multiattack.AttackNumber);
        }

        public static Action Convert(SaveAction saveAction) => saveAction.ActionType switch
        {
            SaveAttackName => new Attack(saveAction.Name, saveAction.Note, saveAction.AttackType, saveAction.AttackMod, saveAction.AdvantageType, saveAction.Range, saveAction.MaxRange, saveAction.Target, saveAction.Damages.Select(Convert).ToArray(),Convert(saveAction.SavingThrow)),
            SaveSavingThrowName => new SavingThrowAction(saveAction.Name, saveAction.Note, Convert(saveAction.SavingThrow)),
            SaveMultiattackName => new Multiattack(saveAction.Name, saveAction.Note, saveAction.MultiAttacks.Select(Convert).ToArray()),
            SaveActionName => new Action(saveAction.Name, saveAction.Note),
            _ => new Action(saveAction.Name, saveAction.Note),
        };

        public static Attack.Damage Convert(SaveDamage damage)
        {
            return new Attack.Damage(new Dice(damage.Number, damage.Faces, damage.Modifiers), damage.DamageType);
        }

        public static SavingThrow Convert (SaveSavingThrow savingThrow)
        {
            return savingThrow.IsGrapple
                ? new SavingThrow("", true, savingThrow.DC, ConvertFailure(savingThrow.FailureSavingThrowOutcome), ConvertSuccess(savingThrow.SuccessSavingThrowOutcome))
                : new SavingThrow("", savingThrow.SavingThrow, savingThrow.DC, ConvertFailure(savingThrow.FailureSavingThrowOutcome), ConvertSuccess(savingThrow.SuccessSavingThrowOutcome));
        }

        public static FailSaveOutcome ConvertFailure(SaveThrowOutcome outcome)
        {
            var throwType = System.Enum.TryParse(outcome.SaveType, out FailSavingThrowOutcomes t) ? t : FailSavingThrowOutcomes.Nothing;
            return new FailSaveOutcome(throwType, new Dice(outcome.Damage.Number, outcome.Damage.Faces, outcome.Damage.Modifiers), outcome.Damage.DamageType, outcome.Condition, outcome.OutcomeNote);
        }

        public static SuccessSaveOutcome ConvertSuccess(SaveThrowOutcome outcome)
        {
            var throwType = System.Enum.TryParse(outcome.SaveType, out SuccessSavingThrowOutcomes t) ? t : SuccessSavingThrowOutcomes.Nothing;
            return new SuccessSaveOutcome(throwType, new Dice(outcome.Damage.Number, outcome.Damage.Faces, outcome.Damage.Modifiers), outcome.Damage.DamageType, outcome.Condition, outcome.OutcomeNote);
        }

        public static Multiattack.MultiattackInfo Convert (SaveMultiAttack multiAttack)
        {
            return new Multiattack.MultiattackInfo(multiAttack.AttackIndex, multiAttack.AttackNumber);
        }

        public string GetFullPathName()
        {
#if PLATFORM_ANDROID && !UNITY_EDITOR
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
            }
            catch (System.Exception e)
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