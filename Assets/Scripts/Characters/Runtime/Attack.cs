using SummonsTracker.Rolling;
using SummonsTracker.Text;
using System.Linq;
using System.Text;

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

        public Attack(string name, string note, AttackType attackType, int attackMod, AdvantageType advantageType, int range, int maxRange, string target, Damage[] damages, SavingThrow savingThrow) : base(name, note)
        {
            AttackType = attackType;
            AttackMod = attackMod;
            AdvantageType = advantageType;
            Range = range;
            MaxRange = maxRange;
            Target = target;
            Damages = damages;
            SavingThrow = savingThrow;
        }

        public int Roll()
        {
            return DiceUtility.Roll(1, 20, AttackMod, AdvantageType);
        }

        public override string ToString()
        {
            var b = new StringBuilder();

            b.Append("<b>").Append(Name).Append("</b>. ");
            b.Append("<i>").Append(TextUtils.DeCamelCase(AttackType.ToString())).Append(":</i> ");
            b.Append(TextUtils.AddPlus(AttackMod)).Append(" to hit, ").Append(Target).Append(". ");
            b.Append("<i>Hit:</i> ").Append(string.Join(", ", Damages.Select(d => d.ToString())));
            b.Append(". ");
            if (SavingThrow != null)
            {
                var savingThrowStr = SavingThrowHelper.SavingThrowToString(SavingThrow).Trim();
                b.Append(savingThrowStr);
                if (!savingThrowStr.EndsWith("."))
                {
                    b.Append(". ");
                }
            }
            var note = Note.Trim();
            b.Append(note);
            if (!note.EndsWith("."))
            {
                b.Append(".");
            }
            return b.ToString().Trim();
        }
    }
}