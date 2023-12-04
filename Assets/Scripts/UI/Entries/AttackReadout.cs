using SummonsTracker.Characters;
using SummonsTracker.Rolling;
using SummonsTracker.Text;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class AttackReadout : Readout
    {
        public Attack Attack => _attack;

        public void Initialise(Attack attack, AdvantageType advantage, int attackRoll, int[] damageRolls, string note)
        {
            _attack = attack;
            for (int i = 0; i < attack.Damages.Length; i++)
            {
                var instGO = GameObject.Instantiate(_damageRollPrefab, _damageRollParent);
                instGO.transform.SetSiblingIndex(i);
                var texts = instGO.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = damageRolls[i].ToString();
                texts[1].text = TextUtils.DeCamelCase(attack.Damages[i].DamageType.ToString());
                instGO.transform.SetAsLastSibling();
            }

            TitleText.text = attack.Name;
            NoteText.gameObject.SetActive(!string.IsNullOrEmpty(note));
            NoteText.text = note;
            NoteText.transform.SetAsLastSibling();
            _attackRollText.text = attackRoll.ToString();
            _advantage.SetActive(advantage == AdvantageType.Advantage);
            _disadvantage.SetActive(advantage == AdvantageType.Disadvantage);
        }
        private Attack _attack;

        [SerializeField]
        private TextMeshProUGUI _attackRollText;
        [SerializeField]
        private RectTransform _damageRollParent;
        [SerializeField]
        private GameObject _damageRollPrefab;
        [Space]

        [SerializeField]
        private GameObject _advantage;
        [SerializeField]
        private GameObject _disadvantage;
    }
}