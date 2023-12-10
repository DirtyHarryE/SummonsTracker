using SummonsTracker.Characters;
using SummonsTracker.Manager;
using SummonsTracker.Text;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public class SummonsPanel : Panel
    {
        public void DoSummon()
        {
            var mainPanel = GameManager.Instance.MainScene;
            mainPanel.SummonCharacters(GetCharacters());
            mainPanel.Open();
        }

        public void RefineSearch(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                foreach (var summonButton in _summonButtons)
                {
                    summonButton.gameObject.SetActive(true);
                }
                foreach (var headerPair in _headers)
                {
                    headerPair.Value.gameObject.SetActive(true);
                }
            }
            else
            {
                var creatures = new List<CreatureType>();
                foreach (var summonButton in _summonButtons)
                {
                    if (ShouldShow(search, summonButton))
                    {
                        summonButton.gameObject.SetActive(true);
                        if (!creatures.Contains(summonButton.CharacterData.Creature))
                        {
                            creatures.Add(summonButton.CharacterData.Creature);
                        }
                    }
                    else
                    {
                        summonButton.gameObject.SetActive(false);
                    }
                }
                foreach (var headerPair in _headers)
                {
                    headerPair.Value.SetActive(creatures.Contains(headerPair.Key));
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();


            foreach (var group in CharacterData.AllCharacters.GroupBy(d => d.Creature).OrderByDescending(g => g.Any(gi => gi.Name.ToLower().Contains("test"))))
            {
                var header = GameObject.Instantiate(_summonTypeText, _content);
                header.GetComponent<TextMeshProUGUI>().text = TextUtils.DeCamelCase(group.Key.ToString());
                _headers.Add(group.Key, header);
                foreach (var c in group.OrderBy(gi => gi.Name.ToLower().Contains("test")).ThenBy(gi => gi.Name))
                {
                    _summonButtons.Add(MakeButton(c));
                }
            }
            OnSliderUpdate(1);
            _slider.value = 1;
        }

        private void OnEnable()
        {
            _slider.onValueChanged.AddListener(OnSliderUpdate);
            OnButtonClick(_summonButtons[0]);
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(OnSliderUpdate);
        }

        private SummonButton MakeButton(CharacterData characterData)
        {
            var instGO = GameObject.Instantiate(_summonButtonPrefab, _content);
            var button = instGO.GetComponent<SummonButton>();
            button.Initialise(characterData, OnButtonClick);
            return button;
        }

        private void OnButtonClick(SummonButton button)
        {
            foreach (var b in _summonButtons)
            {
                b.UnSelect();
            }
            button.Select();
            _data = button.CharacterData;
        }

        private void OnSliderUpdate(float level)
        {
            _summonNumber = Mathf.RoundToInt(level);
            _numberText.text = _summonNumber.ToString();
        }

        private IEnumerable<Character> GetCharacters()
        {
            for (int i = 0; i < _summonNumber; i++)
            {
                yield return new Character(_data);
            }
        }

        private bool ShouldShow(string search, SummonButton summonButton)
        {
            if (string.IsNullOrEmpty(search))
            {
                return true;
            }
            var s = search.ToLower();
            if (summonButton.CharacterData.Name.ToLower().Contains(s))
            {
                return true;
            }
            if (summonButton.CharacterData.Creature.ToString().ToLower().Contains(s))
            {
                return true;
            }

            return false;
        }

        private CharacterData _data;
        private int _summonNumber;

        private Dictionary<CreatureType, GameObject> _headers = new Dictionary<CreatureType, GameObject>();
        private List<SummonButton> _summonButtons = new List<SummonButton>();

        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private TextMeshProUGUI _numberText;
        [SerializeField]
        private RectTransform _content;
        [SerializeField]
        private GameObject _summonTypeText;
        [SerializeField]
        private GameObject _summonButtonPrefab;
    }
}