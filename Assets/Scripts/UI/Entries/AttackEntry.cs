using SummonsTracker.Characters;
using SummonsTracker.Rolling;
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
        public delegate void OnNewTargetMadeDelegate(System.Action<string, bool> onTargetMade);

        public Character Character => _character;
        public System.Action<object> OnValueChanged;
        public System.Action<int> OnTargetChanged;
        public System.Action<AdvantageType> OnAdvantageChanged;

        public abstract void SetValue(object obj, bool update = true);

        public abstract IEnumerable<Action> GetActions();

        public virtual void Initialise(Character character, string title, OnNewTargetMadeDelegate onNewTargetMade)
        {
            _character = character;
            _title.text = title;

            _targetsDropdown.onValueChanged.AddListener(OnTargetDropdownChanged);
            PopulateTargetsDropdown();

            _rollTypeDropdown.onValueChanged.AddListener(OnAdvantageDropdownChanged);
            PopulateRollTypeDropdown();

            _onNewTarget = onNewTargetMade;
        }

        public void ShowExpandButton(bool enabled)
        {
            _expandCollapseButton.gameObject.SetActive(enabled);
        }

        public void SetExpandAction(UnityAction onExpand)
        {
            _expandCollapseButton.onClick.AddListener(onExpand);
        }

        public AdvantageType GetAdvantage() => InternalGetAdvantage(_targetsDropdown.value);

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

        public void SetAdvantage(AdvantageType advantageType)
        {
            var names = System.Enum.GetNames(typeof(AdvantageType));
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == advantageType.ToString())
                {
                    _rollTypeDropdown.SetValueWithoutNotify(i);
                    return;
                }
            }
            _rollTypeDropdown.SetValueWithoutNotify((int)advantageType);
        }

        public void ChangeTarget(int target)
        {
            _targetsDropdown.SetValueWithoutNotify(target);
        }

        public void EnableBackground(bool enabled) => _background.enabled = enabled;

        #region Unity Messages

        private void Awake()
        {
            _repopulateTargets += OnRepopulateTargets;
        }

        private void OnDestroy()
        {
            _repopulateTargets -= OnRepopulateTargets;
        }

        #endregion

        #region Private

        private static event System.Action<AttackEntry> _repopulateTargets;

        private void PopulateTargetsDropdown()
        {
            _targetsDropdown.ClearOptions();
            _targetsDropdown.AddOptions(GetOptions().ToList());

        }

        private void PopulateRollTypeDropdown()
        {
            _rollTypeDropdown.ClearOptions();
            var list = new List<TMP_Dropdown.OptionData>();
            foreach (var adv in System.Enum.GetNames(typeof(Rolling.AdvantageType)))
            {
                list.Add(new TMP_Dropdown.OptionData(adv));
            }
            _rollTypeDropdown.AddOptions(list);
            SetAdvantage(AdvantageType.None);
        }

        private AdvantageType InternalGetAdvantage(int index)
        {
            if (System.Enum.TryParse<AdvantageType>(_targetsDropdown.options[index].text, out var adv))
            {
                return adv;
            }
            return AdvantageType.None;
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
                        _onNewTarget?.Invoke(OnNewTargetMade);
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

        private void OnNewTargetMade(string newName, bool added)
        {
            if (added)
            {
                var list = new List<SaveTarget>(SaveManager.Instance.CurrentProfile.SavedTargets)
                {
                    newName
                };
                SaveManager.Instance.CurrentProfile.SavedTargets = list.ToArray();

                SaveManager.Instance.Save();
                PopulateTargetsDropdown();
                var t = _targetsDropdown.options.Count - 2;
                _targetsDropdown.SetValueWithoutNotify(t);
                OnTargetChanged?.Invoke(t);

                _repopulateTargets?.Invoke(this);
            }
        }

        private void OnAdvantageDropdownChanged(int advantageIndex)
        {
            var adv = GetAdvantage();
            OnAdvantageChanged?.Invoke(adv);
        }

        private void OnRepopulateTargets(AttackEntry caller)
        {
            if (this != caller)
            {
                var index = _targetsDropdown.value;
                var text = _targetsDropdown.options[index].text;
                PopulateTargetsDropdown();
                var found = false;
                for (int i = 0; i < _rollTypeDropdown.options.Count; i++)
                {
                    if (_rollTypeDropdown.options[i].text == text)
                    {
                        _rollTypeDropdown.SetValueWithoutNotify(i);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    _targetsDropdown.SetValueWithoutNotify(index);
                }
            }
        }

        private OnNewTargetMadeDelegate _onNewTarget;

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
        #endregion
    }
}