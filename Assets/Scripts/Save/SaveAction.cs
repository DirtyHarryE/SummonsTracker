using SummonsTracker.Characters;
using SummonsTracker.Rolling;
using SummonsTracker.Text;
using System;
using System.Linq;
using System.Text;

namespace SummonsTracker.Save
{
    [Serializable]
    public class SaveAction
    {
        public string ActionType = "none";

        public string Name = "name";
        public string Note = "note";

        public AttackType AttackType = AttackType.MeleeWeaponAttack;
        public int AttackMod = 0;
        public AdvantageType AdvantageType = AdvantageType.None;
        public int Range = 0;
        public int MaxRange = 0;
        public string Target = "none";
        public SaveDamage[] Damages = Array.Empty<SaveDamage>();
        public SaveSavingThrow SavingThrow = SaveSavingThrow.None;
        public SaveMultiAttack[] MultiAttacks = Array.Empty<SaveMultiAttack>();

        public SaveAction(string actionType, string name, string note)
        {
            ActionType = actionType;
            Name = name;
            Note = note;
        }

        public SaveAction(string actionType, string name, string note, SaveSavingThrow savingThrow) : this(actionType, name, note)
        {
            SavingThrow = savingThrow;
        }

        public SaveAction(string actionType, string name, string note, SaveMultiAttack[] multiAttacks) : this(actionType, name, note)
        {
            MultiAttacks = multiAttacks;
        }

        public SaveAction(string actionType, string name, string note, AttackType attackType, int attackMod, AdvantageType advantageType, int range, int maxRange, string target, SaveDamage[] damages, SaveSavingThrow savingThrow) : this(actionType, name, note)
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

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Type = ").Append(TextUtils.DeCamelCase(ActionType)).Append(", Name = ").Append(Name);
            switch (ActionType)
            {
                case SaveParser.SaveAttackName:
                {
                    builder.Append(", Attack Type = ").Append(AttackType);
                    builder.Append(", Attack Mod = ").Append(AttackMod);
                    builder.Append(", Advantage Type = ").Append(AdvantageType);
                    builder.Append(", Range = ").Append(Range);
                    builder.Append(", Max Range = ").Append(MaxRange);
                    builder.Append(", Max Range = ").Append(MaxRange);
                    builder.Append(", Target = ").Append(Target);
                    builder.Append(", Damage = [");
                    builder.Append(string.Join(", ", Damages.Select(d => $"({d})").ToArray()));
                    builder.Append("], Saving Throw = (").Append(SavingThrow).Append(")");
                    break;
                }
                case SaveParser.SaveMultiattackName:
                {
                    builder.Append(", Multiattack = [");
                    builder.Append(string.Join(", ", MultiAttacks.Select(m => $"({m})").ToArray()));
                    builder.Append("]");
                    break;
                }
                case SaveParser.SaveSavingThrowName:
                {
                    builder.Append(", Saving Throw = (").Append(SavingThrow).Append(")");
                    break;
                }
            }
            builder.Append(", Note = ").Append(Note);
            return builder.ToString();
        }
    }
}