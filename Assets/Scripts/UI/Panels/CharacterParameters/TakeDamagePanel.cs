using SummonsTracker.Characters;
using SummonsTracker.Text;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SummonsTracker.UI
{
    public class TakeDamagePanel : CharacterParameterPanel
    {
        public override void Apply()
        {
            if (int.TryParse(_input.text, out var amount) && amount > 0)
            {
                if(Character.TakeDamage(_damages[_dropDown.value], amount))
                {
                    Character.SetCondition(ConditionTypes.Unconscious);
                }
            }
            base.Apply();
        }

        public override void Open(Character character)
        {
            base.Open(character);
            ValidateDamageDropdown(_dropDown.value);
            EventSystem.current.SetSelectedGameObject(_input.gameObject);
        }

        protected override void Awake()
        {
            base.Awake();

            _damages = GetDamageTypes().ToArray();

            var i = _damages.Select((d, i) => new { D = d, I = i }).First(v => v.D == DamageTypes.Bludgeoning).I;

            _dropDown.options = _damages.Select(s => new TMP_Dropdown.OptionData(TextUtils.DeCamelCase(s.ToString()))).ToList();
            _dropDown.SetValueWithoutNotify(GetPhysical(_damages));
            _dropDown.onValueChanged.AddListener(ValidateDamageDropdown);
            _input.onValueChanged.AddListener(ValidateDamageAmount);
        }

        private int GetPhysical(DamageTypes[] damages)
        {
            for (int i = 0; i < damages.Length; i++)
            {
                if ((damages[i] & (DamageTypes.Bludgeoning | DamageTypes.Slashing | DamageTypes.Piercing)) != 0)
                {
                    return i;
                }
            }
            return 0;
        }

        private void ValidateDamageAmount(string value)
        {
            if (int.TryParse(value, out var amount))
            {
                ValidateDamage(amount, _dropDown.value);
            }
        }

        private void ValidateDamageDropdown(int value)
        {
            if (int.TryParse(_input.text, out var amount))
            {
                ValidateDamage(amount, value);
            }
        }
        
        private void ValidateDamage(int damageAmount, int dropdownValue){

            var damage = _damages[dropdownValue];
            var list = new List<string>();
            if (Character.IsImmune(damage))
            {
                list.Add($"{Character.Name} is immune to {damage}.");
            }
            else if (Character.IsResistant(damage))
            {
                list.Add($"{Character.Name} is resistant to {damage}.");
            }
            else if (Character.Hitpoints + Character.TemporaryHitPoints < damageAmount)
            {
                list.Add($"{Character.Name} will be knocked unconcious!");
            }

            _readoutText.text = string.Join("\n", list.ToArray());
            _readoutText.gameObject.SetActive(list.Count > 0);
        }

        private IEnumerable<DamageTypes> GetDamageTypes()
        {
            foreach (var d in System.Enum.GetValues(typeof(DamageTypes)))
            {
                var damage = (DamageTypes)d;
                if (damage != DamageTypes.none)
                {
                    yield return damage;
                }
            }
        }

        private DamageTypes[] _damages = new DamageTypes[0];

        [Space]
        [SerializeField]
        private TMP_InputField _input;
        [SerializeField]
        private TMP_Dropdown _dropDown;
        [SerializeField]
        private TextMeshProUGUI _readoutText;
    }
}