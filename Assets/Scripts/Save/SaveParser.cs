using SummonsTracker.Characters;
using SummonsTracker.Rolling;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Save
{
    public static class SaveParser
    {
        public const string SaveAttackName = "Attack";
        public const string SaveSavingThrowName = "SavingThrow";
        public const string SaveMultiattackName = "Multiattack";
        public const string SaveActionName = "Action";

        public static SaveAction Convert(Action action)
        {
            if (action is Attack attack)
            {
                return new SaveAction(SaveAttackName, attack.Name, attack.Note, attack.AttackType, attack.AttackMod,
                                      attack.AdvantageType, attack.Range, attack.MaxRange, attack.Target,
                                      attack.Damages.Select(Convert).ToArray(), Convert(attack.SavingThrow));
            }
            else if (action is SavingThrowAction savingThrow)
            {
                return new SaveAction(SaveSavingThrowName, savingThrow.Name, savingThrow.Note, Convert(savingThrow: savingThrow));
            }
            else if (action is Multiattack multiattack)
            {
                return new SaveAction(SaveMultiattackName, multiattack.Name, multiattack.Note, multiattack.Attacks.Select(Convert).ToArray());
            }
            else
            {
                return new SaveAction(SaveActionName, action.Name, action.Note);
            }
        }

        public static SaveDamage Convert(Attack.Damage damage)
        {
            return new SaveDamage(damage.DamageDice.Number, damage.DamageDice.Faces, damage.DamageDice.Modifiers, damage.DamageType);
        }

        public static SaveDamage Convert(DiceBase dice, DamageTypes damageType)
        {
            return new SaveDamage(dice.Number, dice.Faces, dice.Modifiers, damageType);
        }

        public static SaveSavingThrow Convert(ISavingThrow savingThrow)
        {
            return new SaveSavingThrow(savingThrow.IsGrapple,
                savingThrow.SavingThrow,
                savingThrow.DC,
                Convert(savingThrow.FailureSavingThrowOutcome),
                Convert(savingThrow.SuccessSavingThrowOutcome));
        }

        public static SaveThrowOutcome Convert(SuccessSaveOutcome success)
        {
            return new SaveThrowOutcome(success.SuccessSaveType.ToString(), Convert(success.Damage, success.DamageType), success.Condition, success.OutcomeNote);
        }

        public static SaveThrowOutcome Convert(FailSaveOutcome fail)
        {
            return new SaveThrowOutcome(fail.FailSaveType.ToString(), Convert(fail.Damage, fail.DamageType), fail.Condition, fail.OutcomeNote);
        }

        public static SaveMultiAttack Convert(Multiattack.MultiattackInfo multiattack)
        {
            return new SaveMultiAttack(multiattack.AnyAttack ? -1 : multiattack.AttackIndex, multiattack.AttackNumber);
        }

        public static Action Convert(SaveAction saveAction) => saveAction.ActionType switch
        {
            SaveAttackName => new Attack(saveAction.Name, saveAction.Note, saveAction.AttackType, saveAction.AttackMod, saveAction.AdvantageType, saveAction.Range, saveAction.MaxRange, saveAction.Target, saveAction.Damages.Select(Convert).ToArray(), Convert(saveAction.SavingThrow)),
            SaveSavingThrowName => new SavingThrowAction(saveAction.Name, saveAction.Note, Convert(saveAction.SavingThrow)),
            SaveMultiattackName => new Multiattack(saveAction.Name, saveAction.Note, saveAction.MultiAttacks.Select(Convert).ToArray()),
            SaveActionName => new Action(saveAction.Name, saveAction.Note),
            _ => new Action(saveAction.Name, saveAction.Note),
        };

        public static Attack.Damage Convert(SaveDamage damage)
        {
            return new Attack.Damage(new Dice(damage.Number, damage.Faces, damage.Modifiers), damage.DamageType);
        }

        public static SavingThrow Convert(SaveSavingThrow savingThrow)
        {
            return savingThrow.IsGrapple
                ? new SavingThrow("", true, savingThrow.DC, ConvertFailure(savingThrow.FailureSavingThrowOutcome), ConvertSuccess(savingThrow.SuccessSavingThrowOutcome))
                : new SavingThrow("", savingThrow.SavingThrow, savingThrow.DC, ConvertFailure(savingThrow.FailureSavingThrowOutcome), ConvertSuccess(savingThrow.SuccessSavingThrowOutcome));
        }

        public static FailSaveOutcome ConvertFailure(SaveThrowOutcome outcome)
        {
            var throwType = System.Enum.TryParse(outcome.SaveType, out FailSavingThrowOutcomes t) ? t : FailSavingThrowOutcomes.Nothing;
            return new FailSaveOutcome(throwType, new Dice(outcome.Damage.Number, outcome.Damage.Faces, outcome.Damage.Modifiers), outcome.Damage.DamageType, outcome.Condition, outcome.OutcomeNote);
        }

        public static SuccessSaveOutcome ConvertSuccess(SaveThrowOutcome outcome)
        {
            var throwType = System.Enum.TryParse(outcome.SaveType, out SuccessSavingThrowOutcomes t) ? t : SuccessSavingThrowOutcomes.Nothing;
            return new SuccessSaveOutcome(throwType, new Dice(outcome.Damage.Number, outcome.Damage.Faces, outcome.Damage.Modifiers), outcome.Damage.DamageType, outcome.Condition, outcome.OutcomeNote);
        }

        public static Multiattack.MultiattackInfo Convert(SaveMultiAttack multiAttack)
        {
            return new Multiattack.MultiattackInfo(multiAttack.AttackIndex, multiAttack.AttackNumber);
        }

    }
}