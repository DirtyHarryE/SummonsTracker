using SummonsTracker.Characters;
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
        public System.Action<Rolling.AdvantageType> OnAdvantageChanged;

        public abstract object GetValue();

        public abstract void SetValue(object obj, bool update = true);

        public abstract IEnumerable<Action> GetActions();

        public virtual void Initialise(Character character, string title)
        {
            _character = character;
            _title.text = title;

            _advantageButton.Initialise(AdvantageButton);
            _straightRollButton.Initialise(StraightRollButton);
            _disadvantageButton.Initialise(DisadvantageButton);

            _straightRollButton.Select();
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
            if (_advantageButton.Selected) 
                return Rolling.AdvantageType.Advantage;
            if (_disadvantageButton.Selected) 
                return Rolling.AdvantageType.Disadvantage;
            return Rolling.AdvantageType.None;
        }

        public void SetAdvantage(Rolling.AdvantageType advantageType)
        {
            switch (advantageType)
            {
                case Rolling.AdvantageType.None:
                    _advantageButton.UnSelect();
                    _straightRollButton.Select();
                    _disadvantageButton.UnSelect();
                    break;
                case Rolling.AdvantageType.Advantage:
                    _advantageButton.Select();
                    _straightRollButton.UnSelect();
                    _disadvantageButton.UnSelect();
                    break;
                case Rolling.AdvantageType.Disadvantage:
                    _advantageButton.UnSelect();
                    _straightRollButton.UnSelect();
                    _disadvantageButton.Select();
                    break;
            }
            OnAdvantageChanged?.Invoke(advantageType);
        }

        public void EnableBackground(bool enabled) => _background.enabled = enabled;

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
        private ToggleableButton _advantageButton;
        [SerializeField]
        private ToggleableButton _straightRollButton;
        [SerializeField]
        private ToggleableButton _disadvantageButton;
    }
}