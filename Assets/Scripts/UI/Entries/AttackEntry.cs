using SummonsTracker.Characters;
using SummonsTracker.Save;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public abstract class AttackEntry : MonoBehaviour
    {
        public Character Character => _character;
        public System.Action<object> OnValueChanged;
        public System.Action<int> OnTargetChanged;
        public System.Action<Rolling.AdvantageType> OnAdvantageChanged;

        public abstract object GetValue();

        public abstract void SetValue(object obj, bool update = true);

        public abstract IEnumerable<Action> GetActions();

        public virtual void Initialise(Character character, string title)
        {
            _character = character;
            _title.text = title;

            _targetsDropdown.onValueChanged.AddListener(OnTargetDropdownChanged);
            PopulateTargetsDropdown();

            _targetsDropdown.onValueChanged.AddListener(OnAdvantageDropdownChanged);
        }
        public void ShowExpandButton(bool enabled)
        {
            _expandCollapseButton.gameObject.SetActive(enabled);
        }

        public void SetExpandAction(UnityAction onExpand)
        {
            _expandCollapseButton.onClick.AddListener(onExpand);
        }

        public Rolling.AdvantageType GetAdvantage()
        {
            var text = _rollTypeDropdown.options[_rollTypeDropdown.value].text.Trim().ToLower();
            if (text.Contains("disadvantage") || text.Contains("dis"))
            {
                return Rolling.AdvantageType.Disadvantage;
            }
            if (text.Contains("advantage") || text.Contains("adv"))
            {
                return Rolling.AdvantageType.Advantage;
            }
            return _rollTypeDropdown.value switch
            {
                0 => Rolling.AdvantageType.Advantage,
                1 => Rolling.AdvantageType.None,
                2 => Rolling.AdvantageType.Disadvantage,
                _ => Rolling.AdvantageType.None
            };
        }

        public SaveTarget GetTarget()
        {
            var index = _targetsDropdown.value;
            if (index <= 0)
            {
                return SaveTarget.None;
            }
            else
            {
                index -= 1;
                if (index < SaveManager.Instance.CurrentProfile.SavedTargets.Length)
                {
                    return SaveManager.Instance.CurrentProfile.SavedTargets[index];
                }
            }
            return null;
        }

        public void SetAdvantage(Rolling.AdvantageType advantageType)
        {
            for (int i = 0; i < _rollTypeDropdown.options.Count; i++)
            {
                var text = _rollTypeDropdown.options[i].text.Trim().ToLower();
                switch (advantageType)
                {
                    case Rolling.AdvantageType.Advantage:
                    {

                        if (text.Contains("advantage") || text.Contains("adv"))
                        {
                            _rollTypeDropdown.SetValueWithoutNotify(i);
                            return;
                        }
                        break;
                    }
                    case Rolling.AdvantageType.None:
                    {
                        if (text.Contains("straight") || text.Contains("none"))
                        {
                            _rollTypeDropdown.SetValueWithoutNotify(i);
                            return;
                        }
                        return;
                    }
                    case Rolling.AdvantageType.Disadvantage:
                    {
                        if (text.Contains("disadvantage") || text.Contains("dis"))
                        {
                            _rollTypeDropdown.SetValueWithoutNotify(i);
                            return;
                        }
                        break;
                    }
                }
            }

            switch (advantageType)
            {
                case Rolling.AdvantageType.Advantage:
                {
                    _rollTypeDropdown.SetValueWithoutNotify(0);
                    return;
                }
                case Rolling.AdvantageType.None:
                {
                    _rollTypeDropdown.SetValueWithoutNotify(1);
                    return;
                }
                case Rolling.AdvantageType.Disadvantage:
                {
                    _rollTypeDropdown.SetValueWithoutNotify(2);
                    return;
                }
                default:
                {
                    _rollTypeDropdown.SetValueWithoutNotify(1);
                    return;
                }
            }
        }

        public void ChangeTarget(int target)
        {
            _targetsDropdown.SetValueWithoutNotify(target);
        }

        public void EnableBackground(bool enabled) => _background.enabled = enabled;

        private void PopulateTargetsDropdown()
        {
            _targetsDropdown.ClearOptions();
            _targetsDropdown.AddOptions(GetOptions().ToList());

        }

        private IEnumerable<string> GetOptions()
        {
            yield return "No Target";

            if (SaveManager.Instance != null &&
                SaveManager.Instance.CurrentProfile != null &&
                SaveManager.Instance.CurrentProfile.SavedTargets != null)
            {
                foreach (var target in SaveManager.Instance.CurrentProfile.SavedTargets)
                {
                    yield return target.TargetName;
                }
            }
            yield return "New...";
        }

        private void OnTargetDropdownChanged(int target)
        {
            if (target == _targetsDropdown.options.Count - 1)
            {
                if (SaveManager.Instance != null &&
                    SaveManager.Instance.CurrentProfile != null)
                {
                    if (SaveManager.Instance.CurrentProfile.SavedTargets != null)
                    {
                        var list = new List<SaveTarget>(SaveManager.Instance.CurrentProfile.SavedTargets)
                        {
                            "New Target"
                        };
                        SaveManager.Instance.CurrentProfile.SavedTargets = list.ToArray();
                    }
                    else
                    {
                        SaveManager.Instance.CurrentProfile.SavedTargets = new SaveTarget[] { "New Target" };
                    }
                    SaveManager.Instance.Save();
                    PopulateTargetsDropdown();
                    var t = _targetsDropdown.options.Count - 2;
                    _targetsDropdown.SetValueWithoutNotify(t);
                    OnTargetChanged?.Invoke(t);
                }
            }
            else
            {
                OnTargetChanged?.Invoke(target);
            }
        }

        private void OnAdvantageDropdownChanged(int advantageIndex)
        {
            var text = _rollTypeDropdown.options[advantageIndex].text.Trim().ToLower();
            if (text.Contains("disadvantage") || text.Contains("dis"))
            {
                InternalSetAdvantage(Rolling.AdvantageType.Disadvantage);
                return;
            }
            if (text.Contains("advantage") || text.Contains("adv"))
            {
                InternalSetAdvantage(Rolling.AdvantageType.Advantage);
                return;
            }
            switch (advantageIndex)
            {
                case 0:
                {
                    InternalSetAdvantage(Rolling.AdvantageType.Advantage);
                    return;
                }
                case 1:
                {
                    InternalSetAdvantage(Rolling.AdvantageType.None);
                    return;
                }
                case 2:
                {
                    InternalSetAdvantage(Rolling.AdvantageType.Disadvantage);
                    return;
                }
                default:
                {
                    InternalSetAdvantage(Rolling.AdvantageType.None);
                    return;
                }
            }
        }

        private void InternalSetAdvantage(Rolling.AdvantageType advantageType)
        {
            OnAdvantageChanged?.Invoke(advantageType);
        }

        private void AdvantageButton(ToggleableButton button, bool selected) => SetAdvantage(Rolling.AdvantageType.Advantage);

        private void StraightRollButton(ToggleableButton button, bool selected) => SetAdvantage(Rolling.AdvantageType.None);

        private void DisadvantageButton(ToggleableButton button, bool selected) => SetAdvantage(Rolling.AdvantageType.Disadvantage);

        private Character _character;

        [SerializeField]
        private TextMeshProUGUI _title;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private Button _expandCollapseButton;
        [Space]
        [SerializeField]
        private TMP_Dropdown _targetsDropdown;
        [Space]
        [SerializeField]
        private TMP_Dropdown _rollTypeDropdown;
    }
}