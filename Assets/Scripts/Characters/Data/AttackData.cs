using SummonsTracker.Rolling;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummonsTracker.Characters
{
    public enum AttackType
    {
        MeleeWeaponAttack,
        RangedWeaponAttack,
        RangedSpellAttack
    }
    [ActionMenuItem("Attack/Single Damage/No Saving Throw")]
    public class AttackData : ActionData
    {
        public AttackType AttackType => _attackType;
        public int AttackMod => _attackMod;
        public AdvantageType AdvantageType => _advantageType;
        public int Range => _range;
        public int MaxRange => _maxRange;
        public string Target =>_target;
        public Rolling.Dice Damage => _damage;
        public DamageTypes DamageType => _damageType;

        [SerializeField, FormerlySerializedAs("AttackType")]
        public AttackType _attackType = AttackType.MeleeWeaponAttack;
        [SerializeField, FormerlySerializedAs("AttackMod")]
        public int _attackMod = 1;
        [SerializeField, FormerlySerializedAs("AdvantageType")]
        public AdvantageType _advantageType;
        [SerializeField, FormerlySerializedAs("Range")]
        public int _range = 5;
        [SerializeField, FormerlySerializedAs("MaxRange")]
        public int _maxRange = 5;
        [SerializeField, FormerlySerializedAs("Target")]
        public string _target = "one target";
        [SerializeField, FormerlySerializedAs("Damage")]
        public Rolling.Dice _damage = new Rolling.Dice(1, 8, 0);
        [SerializeField, FormerlySerializedAs("DamageType")]
        public DamageTypes _damageType = DamageTypes.Piercing;

        public override Action Instantiate()
        {
            return new Attack(this);
        }
    }
}