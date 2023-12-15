using SummonsTracker.Spell;
using SummonsTracker.Text;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public class CastSpellPanel : Panel
    {
        public int SpellParameter => _spellParameters.value;
        public int NumberSelected => Mathf.RoundToInt(_numberSlider.value);

        public void SelectSpell(SpellData spellData)
        {
            _spellData = spellData;
            MaxToggle(_MaxEnabled, false);
            this.Open();

            _spellName.text = TextUtils.DeCamelCase(spellData.name);

            _spellLevelSlider.minValue = spellData.Level;
            _spellLevelSlider.maxValue = 9;

            OnLevelSliderChanged(spellData.Level);
            OnSpellParametersUpdate(0);

            _spellLevelSlider.value = spellData.Level;
            _spellParameters.value = 0;

        }

        public void CastSpell()
        {
            if (_spellData is SummonSpellData summonSpellData)
            {
                if (_mainScene.ConcentrationCharacters.Any())
                {
                    _concentrationPanel.Open();
                    _concentrationPanel.OnConfirm = DoSummon;
                }
                else
                {
                    DoSummon();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _MaxEnabled.Initialise(MaxToggle);
        }

        private void OnEnable()
        {
            _spellLevelSlider.onValueChanged.AddListener(OnLevelSliderChanged);
            _spellParameters.onValueChanged.AddListener(OnSpellParametersUpdate);
            _numberSlider.onValueChanged.AddListener(OnNumberSliderChanged);
        }

        private void OnDisable()
        {
            _spellLevelSlider.onValueChanged.RemoveListener(OnLevelSliderChanged);
            _spellParameters.onValueChanged.RemoveListener(OnSpellParametersUpdate);
            _numberSlider.onValueChanged.RemoveListener(OnNumberSliderChanged);
        }

        private void OnLevelSliderChanged(float level)
        {
            var spellLevel = Mathf.RoundToInt(level);
            SetSpellLevel(spellLevel);
            SetSpellParameter(SpellParameter);
        }

        private void OnSpellParametersUpdate(int index)
        {
            SetSpellParameter(index);
        }

        private void SetSpellLevel(int spellLevel)
        {
            _spellLevelText.text = spellLevel.ToString();
            if (_spellData is SummonSpellData summonSpellData)
            {
                var number = summonSpellData.GetNumberOfSummons(spellLevel, SpellParameter);
                _numberSlider.maxValue = number;

                var parameters = summonSpellData.GetSpellParameter(spellLevel);
                if (parameters.Any())
                {
                    _spellParameters.gameObject.SetActive(true);
                    _spellParameters.options = parameters.Select(s => new TMP_Dropdown.OptionData(s)).ToList();
                }
                else
                {
                    _spellParameters.gameObject.SetActive(false);
                }
            }
        }

        private void SetSpellParameter(int spellParameter)
        {
            if (_spellData is SummonSpellData summonSpellData)
            {
                var spellLevel = Mathf.RoundToInt(_spellLevelSlider.value);
                var number = summonSpellData.GetNumberOfSummons(spellLevel, spellParameter);

                _numberSlider.maxValue = number;

                if (!_MaxEnabled.Selected)
                {
                    number = Mathf.Min(number, NumberSelected);
                }

                var summonParameters = summonSpellData.GetPerSummonParameters(spellLevel, spellParameter);
                if (summonParameters.Any())
                {
                    var summonOptions = summonParameters.Select(s => new TMP_Dropdown.OptionData(s)).ToList();

                    for (int i = 0; i < _existingDropdowns.Count; i++)
                    {
                        _existingDropdowns[i].gameObject.SetActive(true);
                        _existingDropdowns[i].options = summonOptions;
                    }
                    for (int i = _existingDropdowns.Count; i < number; i++)
                    {
                        var instGO = GameObject.Instantiate(_summonParametersDropdownPrefab, _parent);
                        var dropdown = instGO.GetComponent<TMP_Dropdown>();
                        dropdown.options = summonOptions;
                        _existingDropdowns.Add(dropdown);
                    }
                    for (int i = number; i < _existingDropdowns.Count; i++)
                    {
                        _existingDropdowns[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    for (int i = 0; i < _existingDropdowns.Count; i++)
                    {
                        _existingDropdowns[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        private int GetSummonParameter(int index)
        {
            if (0 <= index && index <= _existingDropdowns.Count - 1)
            {
                return _existingDropdowns[index].value;
            }
            return 0;
        }

        private void DoSummon()
        {
            if (_spellData is SummonSpellData summonSpellData)
            {
                var characters = summonSpellData.GetCharacters(Mathf.RoundToInt(_spellLevelSlider.value), _spellParameters.value, GetSummonParameter, _MaxEnabled.Selected ? -1 : NumberSelected);
                //Debug.Log(string.Join("\n", characters.GroupBy(c => c.Name).Select(g => $"{g.Key} x{g.Count()}").ToArray()));
                _mainScene.SummonCharacters(characters, _spellData.Concentration);
                _mainScene.Open();
            }
        }

        private void MaxToggle(ToggleableButton button, bool selected)
        {
            if (selected)
            {
                _MaxEnabled.UnSelect();
                _numberSlider.gameObject.SetActive(true);
                _numberReadout.gameObject.SetActive(true);
                _numberReadout.text = _numberSlider.value.ToString();
                _numberSlider.value = _numberSlider.maxValue;
                SetSpellParameter(SpellParameter);
            }
            else
            {
                _MaxEnabled.Select();
                _numberSlider.gameObject.SetActive(false);
                _numberReadout.gameObject.SetActive(false);
            }
        }

        private void OnNumberSliderChanged(float number)
        {
            if (!_MaxEnabled.Selected)
            {
                _numberReadout.text = number.ToString();
            }
            SetSpellParameter(SpellParameter);
        }

        [SerializeField]
        private GameObject _summonParametersDropdownPrefab;
        [Space]
        [SerializeField]
        private TextMeshProUGUI _spellName;
        [SerializeField]
        private RectTransform _parent;
        [SerializeField]
        private TextMeshProUGUI _spellLevelText;
        [SerializeField, FormerlySerializedAs("_slider")]
        private Slider _spellLevelSlider;
        [SerializeField]
        private TMP_Dropdown _spellParameters;
        [SerializeField]
        private MainPanel _mainScene;
        [SerializeField]
        private ConfirmConcentrationPanel _concentrationPanel;
        [Space]
        [SerializeField]
        private ToggleableButton _MaxEnabled;
        [SerializeField]
        private TextMeshProUGUI _numberReadout;
        [SerializeField]
        private Slider _numberSlider;

        private SpellData _spellData;
        private List<TMP_Dropdown> _existingDropdowns = new List<TMP_Dropdown>();
    }
}