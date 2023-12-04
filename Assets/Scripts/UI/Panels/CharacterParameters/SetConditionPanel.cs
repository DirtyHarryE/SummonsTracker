using SummonsTracker.Characters;
using SummonsTracker.Text;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public class SetConditionPanel : CharacterParameterPanel
    {
        public override void Apply()
        {
            Character.SetCondition(_conditions[_dropDown.value]);
            base.Apply();
        }

        protected override void Awake()
        {
            base.Awake();

            _conditions = GetConditionTypes().ToArray();

            _dropDown.options = _conditions.Select(s => new TMP_Dropdown.OptionData(TextUtils.DeCamelCase(s.ToString()))).ToList();
            
            _dropDown.onValueChanged.AddListener(ValidateConditions);
        }

        public override void Open(Character character)
        {
            base.Open(character);
            InitConditionButtons();
            ValidateConditions(_dropDown.value);
        }


        private IEnumerable<ConditionTypes> GetConditionTypes()
        {
            foreach (var c in System.Enum.GetValues(typeof(ConditionTypes)))
            {
                var condition = (ConditionTypes)c;
                if (condition != ConditionTypes.none)
                {
                    yield return condition;
                }
            }
        }

        private void ValidateConditions(int value)
        {
            var condition = _conditions[value];
            if (Character.IsImmune(condition))
            {
                _readoutText.gameObject.SetActive(true);
                _readoutText.text = $"{Character.Name} is immune to {condition}";
            }
            else 
            {
                _readoutText.gameObject.SetActive(false);
            }
        }

        private void InitConditionButtons()
        {
            for (int i = 0; i < _conditionsReadouts.Count; i++)
            {
                GameObject.Destroy(_conditionsReadouts[i].gameObject);
            }
            _bottomLine.gameObject.SetActive(false);
            _conditionsReadouts.Clear();
            foreach (var c in System.Enum.GetValues(typeof(ConditionTypes)))
            {
                var condition = (ConditionTypes)c;
                if ((Character.Conditions & condition) != 0)
                {
                    _bottomLine.gameObject.SetActive(true);
                    var instGo = GameObject.Instantiate(_currentConditionPrefab, _bottomLine.parent);
                    instGo.transform.SetSiblingIndex(_bottomLine.GetSiblingIndex());
                    var text = instGo.GetComponentInChildren<TextMeshProUGUI>();
                    if (text != null)
                    {
                        text.text = TextUtils.DeCamelCase(c.ToString());
                    }
                    var button = instGo.GetComponentInChildren<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(delegate
                        {
                            Character.RemoveCondition(condition);
                            InitConditionButtons();
                        });
                    }
                    _conditionsReadouts.Add(instGo);
                }
            }
        }

        private List<GameObject> _conditionsReadouts = new List<GameObject>();
        private ConditionTypes[] _conditions = new ConditionTypes[0];

        [Space]
        [SerializeField]
        private GameObject _currentConditionPrefab;
        [SerializeField]
        private RectTransform _bottomLine;
        [Space]
        [SerializeField]
        private TMP_Dropdown _dropDown;
        [SerializeField]
        private TextMeshProUGUI _readoutText;
    }
}