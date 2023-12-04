using SummonsTracker.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class RolledSavingThrowUI
    {
        public RolledSavingThrowUI(System.Action updateUI, GameObject readoutPrefab, GameObject selectorPrefab, Transform readoutParent, AttackRollPanel.GetStatParentDelegate getStatParent)
        {
            _updateUI = updateUI;
            _readoutPrefab = readoutPrefab;
            _selectorPrefab = selectorPrefab;
            _readoutParent = readoutParent;
            _getStatParent = getStatParent;
        }

        public SavingThrowSelector OnInitialiseSavingThrow(ISavingThrow savingThrow, string name, Character character, int actionIndex, string note)
        {
            if (savingThrow.SavingThrow == StatType.none)
            {
                return null;
            }
            var guid = System.Guid.NewGuid().ToString();
            var selector = CreateSavingThrowSelector(guid, savingThrow);
            var readout = CreateSavingThrowReadout(savingThrow, name, note, actionIndex);
            _savingThrowReadouts.Add(guid, readout);
            _savingThrowSelectors.Add(guid, selector);

            var failResult = 0;
            var succResult = 0;

            if (savingThrow.FailureSavingThrowOutcome.IsDamage)
            {
                failResult = savingThrow.FailureSavingThrowOutcome.Damage.Roll();
                Debug.Log($"FAILURE: {name} => {failResult}");
            }
            if (savingThrow.SuccessSavingThrowOutcome.IsDamage)
            {
                succResult = savingThrow.SuccessSavingThrowOutcome.Damage.Roll();
                Debug.Log($"SUCCESS: {name} => {succResult}");
            }
            else if (savingThrow.SuccessSavingThrowOutcome.SuccessSaveType == SuccessSavingThrowOutcomes.Half)
            {
                succResult = Mathf.FloorToInt(failResult * 0.5f);
            }

            var rolledSavingThrow = new RolledSavingThrow(guid, savingThrow, character, failResult, succResult, actionIndex);
            _rolledSavingThrows.Add(guid, rolledSavingThrow);

            UpdateSavingThrowReadout(rolledSavingThrow, selector, readout);
            return selector;
        }

        public bool OnInitialiseAttackSavingThrow(Attack attack, string instanceID, Character character, int actionIndex)
        {
            var savingThrow = attack.SavingThrow;
            if (!_savingThrowReadouts.ContainsKey(instanceID))
            {
                var selector = OnInitialiseSavingThrow(savingThrow, attack.Name, character, actionIndex, attack.Note);
                //var selector = CreateSavingThrowSelector(rolledAttack.AttackInstanceGUID, savingThrow);
                if (selector != null)
                {
                    selector.TiedAttack = attack;
                    // _savingThrowReadouts.Add(instanceID, CreateSavingThrowReadout(savingThrow, attack.Name, attack.Note, actionIndex));
                    return true;
                }
            }
            return false;
        }

        public void UpdateSavingThrowReadout(SavingThrowSelector savingThrowSelector)
        {
            if (_rolledSavingThrows.TryGetValue(savingThrowSelector.AttackInstanceGUID, out var rolledSavingThrow))
            {
                if (_savingThrowReadouts.TryGetValue(rolledSavingThrow.SavingThrowInstanceGUID, out var readout))
                {
                    UpdateSavingThrowReadout(rolledSavingThrow, savingThrowSelector, readout);
                }
            }
        }

        public void UpdateSavingThrowReadout(RolledSavingThrow rolledSavingThrow)
        {
            if (_savingThrowReadouts.TryGetValue(rolledSavingThrow.SavingThrowInstanceGUID, out var readout))
            {
                if (_savingThrowSelectors.TryGetValue(rolledSavingThrow.SavingThrowInstanceGUID, out var savingThrowSelector))
                {
                    UpdateSavingThrowReadout(rolledSavingThrow, savingThrowSelector, readout);
                }
            }
        }

        public void UpdateSavingThrowReadout(RolledSavingThrow rolledSavingThrow, SavingThrowSelector savingThrowSelector)
        {
            if (_savingThrowReadouts.TryGetValue(rolledSavingThrow.SavingThrowInstanceGUID, out var readout))
            {
                UpdateSavingThrowReadout(rolledSavingThrow, savingThrowSelector, readout);
            }
        }

        public void UpdateSavingThrowReadout(RolledSavingThrow rolledSavingThrow, SavingThrowSelector savingThrowSelector, SavingThrowReadout savingThrowReadout)
        {
            var amount = 0;
            var damageType = DamageTypes.none;
            var condition = ConditionTypes.none;
            var note = "";
            if (savingThrowSelector.IsLocked)
            {
                savingThrowReadout.IsReadoutActive = false;
                amount = rolledSavingThrow.FailDamageResult;
                damageType = rolledSavingThrow.SavingThrow.FailureSavingThrowOutcome.DamageType;
                condition = rolledSavingThrow.SavingThrow.FailureSavingThrowOutcome.Condition;
                note = rolledSavingThrow.SavingThrow.FailureSavingThrowOutcome.OutcomeNote;
            }
            else
            {
                var madeSavingThrow = savingThrowSelector.IsCrossedOut;

                if (madeSavingThrow)
                {
                    if (rolledSavingThrow.SavingThrow.SuccessSavingThrowOutcome.SuccessSaveType == SuccessSavingThrowOutcomes.Nothing)
                    {
                        savingThrowReadout.IsReadoutActive = false;
                    }
                    else
                    {
                        if (rolledSavingThrow.SavingThrow.SuccessSavingThrowOutcome.SuccessSaveType == SuccessSavingThrowOutcomes.Half)
                        {
                            amount = rolledSavingThrow.SuccessDamageResult;
                            damageType = rolledSavingThrow.SavingThrow.FailureSavingThrowOutcome.DamageType;
                            condition = rolledSavingThrow.SavingThrow.FailureSavingThrowOutcome.Condition;
                            note = rolledSavingThrow.SavingThrow.FailureSavingThrowOutcome.OutcomeNote;
                        }
                        else
                        {
                            amount = rolledSavingThrow.SuccessDamageResult;
                            damageType = rolledSavingThrow.SavingThrow.SuccessSavingThrowOutcome.DamageType;
                            condition = rolledSavingThrow.SavingThrow.SuccessSavingThrowOutcome.Condition;
                            note = rolledSavingThrow.SavingThrow.SuccessSavingThrowOutcome.OutcomeNote;
                        }
                        savingThrowReadout.IsReadoutActive = true;
                    }
                }
                else
                {
                    amount = rolledSavingThrow.FailDamageResult;
                    damageType = rolledSavingThrow.SavingThrow.FailureSavingThrowOutcome.DamageType;
                    condition = rolledSavingThrow.SavingThrow.FailureSavingThrowOutcome.Condition;
                    note = rolledSavingThrow.SavingThrow.FailureSavingThrowOutcome.OutcomeNote;

                    savingThrowReadout.IsReadoutActive = true;
                }
            }
            savingThrowReadout.SetDamageAndCondition(amount, damageType, condition, note);
        }

        public bool UpdateUI(Action<DamageTypes, int> addDamage, Action<ConditionAndOutcome> addCondition)
        {
            var someDamage = false;
            foreach (var pair in _rolledSavingThrows)
            {
                var rolledThrows = pair.Value;
                var savingThrow = rolledThrows.SavingThrow;
                var failOutcome = savingThrow.FailureSavingThrowOutcome;
                var succOutcome = savingThrow.SuccessSavingThrowOutcome;
                //foreach (var selector in _savingThrowSelectors)
                if (_savingThrowSelectors.TryGetValue(pair.Key, out var selector))
                {
                    if (selector.SavingThrow == savingThrow)
                    {
                        if (selector.IsLocked)
                        {
                            continue;
                        }
                        var fail = 0;
                        var succ = 0;
                        var failType = DamageTypes.none;
                        var succType = DamageTypes.none;
                        if (!selector.IsCrossedOut)
                        {
                            if (failOutcome.IsDamage)
                            {
                                fail += rolledThrows.FailDamageResult;
                                failType |= failOutcome.DamageType;
                            }
                            if (failOutcome.IsCondition)
                            {
                                addCondition(new ConditionAndOutcome(failOutcome.Condition, failOutcome.OutcomeNote));
                                someDamage = true;
                            }
                        }
                        else
                        {
                            if (failOutcome.IsDamage && succOutcome.SuccessSaveType == SuccessSavingThrowOutcomes.Half)
                            {
                                succ += rolledThrows.SuccessDamageResult;
                                succType |= failOutcome.DamageType;
                            }
                            if (succOutcome.IsDamage)
                            {
                                succ += rolledThrows.SuccessDamageResult;
                                succType |= succOutcome.DamageType;
                            }
                            if (succOutcome.IsCondition)
                            {
                                addCondition(new ConditionAndOutcome(succOutcome.Condition, succOutcome.OutcomeNote));
                                someDamage = true;
                            }
                        }
                        foreach (var enumObj in System.Enum.GetValues(typeof(DamageTypes)))
                        {
                            var damageType = (DamageTypes)enumObj;
                            if (fail > 0 && (failType & damageType) != 0)
                            {
                                //_damageTypeDict[damageType].Number += fail;
                                addDamage(damageType, fail);
                                someDamage = true;
                            }
                            if (succ > 0 && (succType & damageType) != 0)
                            {
                                //_damageTypeDict[damageType].Number += succ;
                                addDamage(damageType, succ);
                                someDamage = true;
                            }
                        }
                    }
                }
            }
            return someDamage;
        }

        public SavingThrowSelector CreateSavingThrowSelector(string attackGUID, ISavingThrow savingThrow)
        {
            var parent = _getStatParent(savingThrow.SavingThrow);
            if (parent == null)
            {
                return null;
            }
            parent.gameObject.SetActive(true);
            var instGO = GameObject.Instantiate(_selectorPrefab, parent);
            var selector = instGO.GetComponent<SavingThrowSelector>();
            selector.Initialise(attackGUID, savingThrow, OnSelectSavingThrow);
            selector.CrossOut(false);
            return selector;
        }

        public SavingThrowReadout CreateSavingThrowReadout(ISavingThrow savingThrow, string name, string note, int attackIndex)
        {
            var instGO = GameObject.Instantiate(_readoutPrefab, _readoutParent);
            var savingThrowReadout = instGO.GetComponent<SavingThrowReadout>();
            savingThrowReadout.Initialise(savingThrow, name, note);
            savingThrowReadout.AttackIndex = attackIndex;
            savingThrowReadout.IsReadoutActive = true;
            return savingThrowReadout;
        }

        public bool TryGetReadout(string instanceID, out SavingThrowReadout savingThrowReadout)
        {
            return _savingThrowReadouts.TryGetValue(instanceID, out savingThrowReadout);
        }

        public IEnumerable<SavingThrowReadout> GetReadouts()
        {
            foreach (var pair in _savingThrowReadouts)
            {
                yield return pair.Value;
            }
        }

        public bool TryGetSelector(ISavingThrow savingThrow, out SavingThrowSelector savingThrowSelector)
        {
            foreach (var pair in _savingThrowSelectors)
            {
                if (pair.Value.SavingThrow == savingThrow)
                {
                    savingThrowSelector = pair.Value;
                    return true;
                }
            }
            savingThrowSelector = null;
            return false;
        }

        public IEnumerable<SavingThrowSelector> GetSavingThrowSelector(Attack attack)
        {
            foreach (var pair in _savingThrowSelectors)
            {
                if (pair.Value.TiedAttack == attack)
                {
                    yield return pair.Value;
                }
            }
            //return _savingThrowSelectors.Where(s => s.Value.TiedAttack == attack);
        }

        public IEnumerable<DamageTypes> GetAllDamageTypes()
        {
            foreach (var pair in _rolledSavingThrows)
            {
                var savingThrow = pair.Value;
                yield return savingThrow.SavingThrow.FailureSavingThrowOutcome.DamageType;
                yield return savingThrow.SavingThrow.SuccessSavingThrowOutcome.DamageType;
            }
        }
        public IEnumerable<ConditionAndOutcome> GetAllConditionAndOutcomes()
        {
            foreach (var pair in _rolledSavingThrows)
            {
                var savingThrow = pair.Value;
                if (savingThrow.SavingThrow.FailureSavingThrowOutcome.IsCondition)
                {
                    yield return new ConditionAndOutcome(savingThrow.SavingThrow.FailureSavingThrowOutcome.Condition, savingThrow.SavingThrow.FailureSavingThrowOutcome.OutcomeNote);
                }
                if (savingThrow.SavingThrow.SuccessSavingThrowOutcome.IsCondition)
                {
                    yield return new ConditionAndOutcome(savingThrow.SavingThrow.SuccessSavingThrowOutcome.Condition, savingThrow.SavingThrow.FailureSavingThrowOutcome.OutcomeNote);
                }
            }
        }

        public void DestroyGameObjects()
        {
            foreach (var selector in _savingThrowSelectors)
            {
                UnityEngine.Object.Destroy(selector.Value.gameObject);
            }
        }

        public void Clear()
        {
            _savingThrowReadouts.Clear();
            _savingThrowSelectors.Clear();
        }

        private void OnSelectSavingThrow(SavingThrowSelector selector)
        {
            var crossOut = !selector.IsCrossedOut;
            selector.CrossOut(crossOut);
            //if (_savingThrowReadoutDict.TryGetValue(selector.AttackInstanceGUID, out var readout))
            //{
            //    readout.IsReadoutActive = !crossOut;
            //}
            UpdateSavingThrowReadout(selector);
            _updateUI();
        }

        private Dictionary<string, RolledSavingThrow> _rolledSavingThrows = new Dictionary<string, RolledSavingThrow>();
        private Dictionary<string, SavingThrowSelector> _savingThrowSelectors = new Dictionary<string, SavingThrowSelector>();
        private Dictionary<string, SavingThrowReadout> _savingThrowReadouts = new Dictionary<string, SavingThrowReadout>();

        private GameObject _readoutPrefab;
        private GameObject _selectorPrefab;
        private System.Action _updateUI;
        private Transform _readoutParent;
        private AttackRollPanel.GetStatParentDelegate _getStatParent;
    }
}