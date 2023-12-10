using SummonsTracker.Rolling;
using SummonsTracker.Text;
using System.Linq;

namespace SummonsTracker.Characters
{
    public class Attack : Action
    {
        public class Damage
        {
            public DiceBase DamageDice;
            public DamageTypes DamageType;

            public Damage(DiceBase damageDice, DamageTypes damageType)
            {
                DamageDice = damageDice;
                DamageType = damageType;
            }

            public override string ToString()
            {
                return $"{DamageDice} {DamageType.ToString().ToLower()} damage";
            }
        }

        public AttackType AttackType;
        public int AttackMod;
        public AdvantageType AdvantageType;
        public int Range;
        public int MaxRange;
        public string Target;
        public Damage[] Damages;
        public SavingThrow SavingThrow;

        public Attack(AttackData attackData) : base(attackData)
        {
            AttackType = attackData.AttackType;
            AttackMod = attackData.AttackMod;
            AdvantageType = attackData.AdvantageType;
            Range = attackData.Range;
            MaxRange = attackData.MaxRange;
            Target = attackData.Target;
            Damages = new[] { new Damage(new Rolling.Dice(attackData.Damage.Number, attackData.Damage.Faces, attackData.Damage.Modifiers), attackData.DamageType) };
        }

        public int Roll()
        {
            return DiceUtility.Roll(1, 20, AttackMod, AdvantageType);
        }

        public override string ToString()
        {
            var atk = $"<b>{Name}</b>. <i>{TextUtils.DeCamelCase(AttackType.ToString())}:</i> {TextUtils.AddPlus(AttackMod)} to hit, {Target}. <i>Hit:</i> {string.Join(", ", Damages.Select(d => d.ToString()))}.";
            if (SavingThrow != null)
            {
                atk = $"{atk} {SavingThrow}";
            }
            atk = $"{atk} {Note}";
            return atk;
        }
    }
}