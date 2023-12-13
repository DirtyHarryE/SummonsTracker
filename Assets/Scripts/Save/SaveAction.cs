using SummonsTracker.Characters;
using SummonsTracker.Rolling;
using SummonsTracker.Text;
using System;
using System.Text;

namespace SummonsTracker.Save
{
    [Serializable]
    public class SaveAction
    {
        public string ActionType;

        public string Name;
        public string Note;

        public AttackType AttackType;
        public int AttackMod;
        public AdvantageType AdvantageType;
        public int Range;
        public int MaxRange;
        public string Target;
        public SaveDamage[] Damages;
        public SaveSavingThrow SavingThrow;

        public SaveMultiAttack[] MultiAttacks;

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

            builder.Append(TextUtils.DeCamelCase(ActionType)).Append(", ").Append(Name);

            switch (ActionType)
            {
                case SaveManager.SaveAttackName:
                {
                    builder.Append(", Attack Type = ").Append(AttackType);
                    builder.Append(", Attack Mod = ").Append(AttackMod);
                    builder.Append(", Advantage Type = ").Append(AdvantageType);
                    builder.Append(", Range = ").Append(Range);
                    builder.Append(", Max Range = ").Append(MaxRange);
                    builder.Append(", Max Range = ").Append(MaxRange);
                    builder.Append(", Target = ").Append(Target);
                    builder.Append(", Damage = [");
                    for (int i = 0; i < Damages.Length; i++)
                    {
                        if (i != 0)
                        {
                            builder.Append(", ");
                        }
                        builder.Append("(");
                        builder.Append(Damages[i].Number).Append("d").Append(Damages[i].Faces).Append(TextUtils.AddPlus(Damages[i].Modifiers)).Append(" ").Append(Damages[i].DamageType);
                        builder.Append(")");
                    }
                    builder.Append("], Saving Throw = (").Append(SavingThrow).Append(")");
                    break;
                }
                case SaveManager.SaveMultiattackName:
                {
                    builder.Append(", Multiattack = [");
                    for (int i = 0; i < MultiAttacks.Length; i++)
                    {
                        if (i != 0)
                        {
                            builder.Append(", ");
                        }
                        builder.Append("(");
                        builder.Append(MultiAttacks[i].AnyAttack ? "Any" : MultiAttacks[i].AttackIndex.ToString()).Append(" => ").Append(MultiAttacks[i].AttackNumber);
                        builder.Append(")");
                    }
                    builder.Append("]");
                    break;
                }
                case SaveManager.SaveSavingThrowName:
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