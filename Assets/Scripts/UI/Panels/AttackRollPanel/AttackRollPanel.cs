using BoilerPlate;
using SummonsTracker.Characters;
using SummonsTracker.Rolling;
using SummonsTracker.Save;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummonsTracker.UI
{
    public class AttackRollPanel : Panel
    {
        public delegate Transform GetStatParentDelegate(StatType statType, bool isGrapple);

        public void Initialise(IEnumerable<CharacterActionInfo> selectedAttacks)
        {
            _targets = selectedAttacks.SelectMany(a => a.Actions).Select(a => a.Target).Distinct().ToArray();
            _targetACs = new int[_targets.Length];
            foreach (var target in SaveManager.Instance.CurrentProfile.SavedTargets)
            {
                for (int j = 0; j < _targets.Length; j++)
                {
                    if (target.GUID == _targets[j].GUID)
                    {
                        _targetACs[j] = target.ExpectedAC;
                        break;
                    }
                }
            }
            
            _currentTargetIndex = Mathf.Clamp(_currentTargetIndex, 0, _targets.Length - 1);
            if (_savingThrowUI == null)
            {
                _savingThrowUI = new RolledSavingThrowUI(UpdateUI, _savingThrowReadoutPrefab, _savingThrowSelectorPrefab, _readoutParent, GetStatParent);
            }
            ResetValues();
            foreach (var selectedAttack in selectedAttacks)
            {
                int actionIndex = 0;
                foreach (var characterAction in selectedAttack.Actions)
                {
                    var action = characterAction.Action;
                    if (action is Multiattack)
                    {
                        continue;
                    }
                    else if (action is Attack attack)
                    {
                        var advantage = GetAdvantageType(characterAction.AdvantageType, attack.AdvantageType);
                        var atkRoll = Dice.D20.Roll(advantage);

                        var damageRollResults = new int[attack.Damages.Length];
                        for (int i = 0; i < damageRollResults.Length; i++)
                        {
                            var damageDice = attack.Damages[i].DamageDice;
                            var dmgRoll = damageDice.Roll();
                            if (atkRoll >= 20)
                            {
                                dmgRoll += damageDice.Number * damageDice.Faces;
                            }
                            damageRollResults[i] = dmgRoll;
                        }
                        var rolledAttack = new RolledAttack(atkRoll, advantage, characterAction.Target, attack, selectedAttack.Character, damageRollResults, actionIndex);
                        _rolledAttacks.Add(rolledAttack);
                        AttackReadout readout = null;
                        if (attack.SavingThrow != null)
                        {
                            if (_savingThrowUI.OnInitialiseAttackSavingThrow(attack, rolledAttack.AttackInstanceGUID, rolledAttack.Character, actionIndex, characterAction.Target))
                            {
                                readout = CreateAttackReadout(rolledAttack, string.Empty);
                            }
                        }
                        else
                        {
                            readout = CreateAttackReadout(rolledAttack, attack.Note);
                        }
                        if (readout != null)
                        {
                            _attackReadoutDict.Add(rolledAttack, readout);
                        }
                    }
                    else if (action is SavingThrowAction savingThrowAction)
                    {
                        _savingThrowUI.OnInitialiseSavingThrow(savingThrowAction, savingThrowAction.Name, characterAction.Target, selectedAttack.Character, actionIndex, savingThrowAction.Note);
                    }
                    else
                    {
                        _actionReadoutDict.Add(action, CreateActionReadout(action, actionIndex));
                    }
                    actionIndex += 1;
                }
            }

            var addedDict = new Dictionary<int, bool>();

            foreach (var groupedAttacks in _rolledAttacks.GroupBy(r => r.Result).OrderBy(g => g.Key))
            {
                var result = groupedAttacks.Key;
                var targets = groupedAttacks.Select(a => a.SaveTarget).Distinct().ToArray();

                foreach (var attack in groupedAttacks)
                {
                    var r = Mathf.Abs(result);
                    if (attack.IsCrit)
                    {
                        r = -r;
                    }
                    if (addedDict.ContainsKey(r))
                    {
                        continue;
                    }
                    var instGO = GameObject.Instantiate(_attackRollSelectorPrefab, _attackRollParent);
                    var selector = instGO.GetComponent<AttackRollSelector>();
                    selector.Initialise(attack.AttackInstanceGUID, result, attack.IsCrit, OnSelectRoll, targets);
                    selector.CrossOut(false);
                    _rollSelectors.Add(selector);
                    addedDict.Add(r, attack.IsCrit);
                }
            }
            var damageTypes = _rolledAttacks.SelectMany(r => r.Attack.Damages)
                                            .Select(d => d.DamageType)
                                            .Concat(_savingThrowUI.GetAllDamageTypes())
                                            .Where(d => d != DamageTypes.none)
                                            .Distinct()
                                            .OrderBy(d => d);
            int index = 0;
            foreach (var damageType in damageTypes)
            {
                var instGO = GameObject.Instantiate(_damageRollPrefab, _readoutParent);
                instGO.transform.SetSiblingIndex(index++);
                var readout = instGO.GetComponent<DamageRollReadout>();
                readout.Number = 0;
                readout.DamageType = damageType;
                _damageTypeDict.Add(damageType, readout);
            }
            var conditionAndOutcomes = _savingThrowUI.GetAllConditionAndOutcomes()
                            .Where(c => c.Condition != ConditionTypes.none)
                            .Distinct()
                            .OrderBy(c => c.Condition);
            foreach (var conditionAndOutcome in conditionAndOutcomes)
            {
                var instGO = GameObject.Instantiate(_conditionReadoutPrefab, _readoutParent);
                instGO.transform.SetSiblingIndex(index++);
                var readout = instGO.GetComponent<ConditionReadout>();
                readout.Condition = conditionAndOutcome.Condition;
                readout.OutcomeNote = conditionAndOutcome.Outcome;
                _conditionDict.Add(conditionAndOutcome, readout);
            }
            UpdateUI();
            base.Open();
        }

        public void TargetLeftButton() => ChangeTarget(-1);

        public void TargetRightButton() => ChangeTarget(1);

        #region Protected

        protected override void OnClose()
        {
            for (int i = 0; i < SaveManager.Instance.CurrentProfile.SavedTargets.Length; i++)
            {
                for (int j = 0; j < _targets.Length; j++)
                {
                    if (SaveManager.Instance.CurrentProfile.SavedTargets[i].GUID == _targets[j].GUID)
                    {
                        SaveManager.Instance.CurrentProfile.SavedTargets[i] = new SaveTarget()
                        {
                            GUID = _targets[j].GUID,
                            TargetName = _targets[j].TargetName,
                            ExpectedAC = _targetACs[j],
                            Fixed = false
                        };
                        break;
                    }
                }
            }
            SaveManager.Instance.Save();
        }

        #endregion

        #region Private

        private AdvantageType GetAdvantageType(AdvantageType a, AdvantageType b)
        {
            if (a == b)
            {
                return a;
            }
            if (a == AdvantageType.None)
            {
                return b;
            }
            if (b == AdvantageType.None)
            {
                return a;
            }
            return AdvantageType.None;
        }

        private void UpdateUI()
        {
            var currentTarget = _targets[_currentTargetIndex];
            _targetReadout.text = currentTarget.TargetName;

            foreach (var otherselector in _rollSelectors)
            {
                if (otherselector.Targets.Contains(currentTarget))
                {
                    otherselector.gameObject.SetActive(true);
                    otherselector.CrossOut(_targetACs[_currentTargetIndex] > otherselector.Result);
                }
                else
                {
                    otherselector.gameObject.SetActive(false);
                }
            }
            foreach (var pair in _damageTypeDict)
            {
                pair.Value.gameObject.SetActive(false);
                pair.Value.Number = 0;
            }
            foreach (var pair in _conditionDict)
            {
                pair.Value.gameObject.SetActive(false);
            }
            var someDamage = false;
            foreach (var rolledAttack in _rolledAttacks)
            {
                if (rolledAttack.SaveTarget != currentTarget)
                {
                    continue;
                }
                var attackHits = rolledAttack.Result >= _targetACs[_currentTargetIndex];
                if (attackHits)
                {
                    for (int i = 0; i < rolledAttack.Attack.Damages.Length; i++)
                    {
                        var number = rolledAttack.DamageRollResults[i];
                        var damage = rolledAttack.Attack.Damages[i];
                        _damageTypeDict[damage.DamageType].Number += number;
                        someDamage = true;
                    }
                }
                if (_attackReadoutDict.TryGetValue(rolledAttack, out var attackReadout))
                {
                    attackReadout.IsReadoutActive = attackHits;
                }
                foreach (var selector in _savingThrowUI.GetSavingThrowSelector(rolledAttack.Attack))
                {
                    selector.Lock(!attackHits);
                    _savingThrowUI.UpdateSavingThrowReadout(selector);
                }
                if (rolledAttack.Attack.SavingThrow != null && _savingThrowUI.TryGetReadout(rolledAttack.AttackInstanceGUID, out var savingThrowReadout))
                {
                    //var savingThrowSelector = _savingThrowSelectors.FirstOrDefault(s => s.SavingThrow == rolledAttack.Attack.SavingThrow);
                    if (_savingThrowUI.TryGetSelector(rolledAttack.Attack.SavingThrow, out var savingThrowSelector))
                    {
                        savingThrowReadout.IsReadoutActive = attackHits && !savingThrowSelector.IsCrossedOut;
                    }
                    else
                    {
                        savingThrowReadout.IsReadoutActive = attackHits;
                    }
                }
            }

            _savingThrowUI.UpdateUI((t, d) =>
            {
                if (d > 0)
                {
                    _damageTypeDict[t].Number += d;
                    someDamage = true;
                }
            }, c =>
            {
                if (_conditionDict.TryGetValue(c, out var readout))
                {
                    readout.gameObject.SetActive(true);
                    someDamage = true;
                }
            }, currentTarget);

            foreach (var damagePair in _damageTypeDict)
            {
                damagePair.Value.gameObject.SetActive(damagePair.Value.Number > 0);
            }

            int activeReadoutIndex = 0;
            int inactiveReadoutIndex = 0;
            var someActive = false;
            var someInactive = false;
            foreach (var readout in GetReadouts().OrderBy(r => r.AttackIndex))
            {
                if (readout.Target != currentTarget)
                {
                    readout.gameObject.SetActive(false);
                }
                else
                {
                    readout.gameObject.SetActive(true);
                    if (readout.IsReadoutActive)
                    {
                        readout.transform.SetSiblingIndex(_activeHeader.GetSiblingIndex() + 1 + (activeReadoutIndex++));
                        readout.OnSetActive();
                        someActive = true;
                    }
                    else
                    {
                        readout.transform.SetSiblingIndex(_inactiveHeader.GetSiblingIndex() + 1 + (inactiveReadoutIndex++));
                        readout.OnSetInactive();
                        someInactive = true;
                    }
                }
            }
            _activeHeader.gameObject.SetActive(someActive);
            _inactiveHeader.gameObject.SetActive(someInactive);

            _noDamage.SetActive(!someDamage);
        }

        private AttackReadout CreateAttackReadout(RolledAttack rolledAttack, string note)
        {
            var instGO = GameObject.Instantiate(_attackReadoutPrefab, _readoutParent);
            instGO.transform.SetSiblingIndex(_activeHeader.GetSiblingIndex() + 1);
            var attackReadout = instGO.GetComponent<AttackReadout>();
            attackReadout.Initialise(rolledAttack.Attack, rolledAttack.Advantage, rolledAttack.SaveTarget, rolledAttack.Result, rolledAttack.DamageRollResults, note);
            attackReadout.AttackIndex = rolledAttack.AttackIndex;
            return attackReadout;
        }

        private ActionReadout CreateActionReadout(Action action, int attackIndex)
        {
            var instGO = GameObject.Instantiate(_actionReadoutPrefab, _readoutParent);
            instGO.transform.SetSiblingIndex(_activeHeader.GetSiblingIndex() + 1);
            var actionReadout = instGO.GetComponent<ActionReadout>();
            actionReadout.Initialise(action);
            actionReadout.AttackIndex = attackIndex;
            actionReadout.IsReadoutActive = true;
            return actionReadout;
        }

        private void OnSelectRoll(AttackRollSelector selector)
        {
            var currentTarget = _targets[_currentTargetIndex];
            if (selector.IsCrossedOut)
            {
                _targetACs[_currentTargetIndex] = selector.Result;
            }
            else
            {
                _targetACs[_currentTargetIndex] = selector.Result + 1;
            }
            UpdateUI();
        }

        private IEnumerable<Readout> GetReadouts()
        {
            foreach (var pair in _attackReadoutDict)
            {
                yield return pair.Value;
            }
            foreach (var savingThrowReadout in _savingThrowUI.GetReadouts())
            {
                yield return savingThrowReadout;
            }
            foreach (var pair in _actionReadoutDict)
            {
                yield return pair.Value;
            }
        }

        private void ResetValues()
        {
            foreach (var selector in _rollSelectors)
            {
                UnityEngine.Object.Destroy(selector.gameObject);
            }
            _savingThrowUI.DestroyGameObjects();
            foreach (var pair in _damageTypeDict)
            {
                UnityEngine.Object.Destroy(pair.Value.gameObject);
            }
            foreach (var readout in GetReadouts())
            {
                UnityEngine.Object.Destroy(readout.gameObject);
            }
            _strengthSavingThrows.gameObject.SetActive(false);
            _dexteritySavingThrows.gameObject.SetActive(false);
            _constitutionSavingThrows.gameObject.SetActive(false);
            _intelligenceSavingThrows.gameObject.SetActive(false);
            _wisdomSavingThrows.gameObject.SetActive(false);
            _charismaSavingThrows.gameObject.SetActive(false);
            _grappleSavingThrows.gameObject.SetActive(false);

            _rollSelectors.Clear();
            _rolledAttacks.Clear();
            _damageTypeDict.Clear();
            _attackReadoutDict.Clear();
            _savingThrowUI.Clear();
            _actionReadoutDict.Clear();
        }

        private Transform GetStatParent(StatType statType, bool isGrapple) => isGrapple ? _grappleSavingThrows : statType switch
        {
            StatType.Strength => _strengthSavingThrows,
            StatType.Dexterity => _dexteritySavingThrows,
            StatType.Constitution => _constitutionSavingThrows,
            StatType.Intelligence => _intelligenceSavingThrows,
            StatType.Wisdom => _wisdomSavingThrows,
            StatType.Charisma => _charismaSavingThrows,
            _ => null
        };

        private void ChangeTarget(int increment)
        {
            _currentTargetIndex = Mathbp.Wrap(_currentTargetIndex + increment, _targets.Length);
            UpdateUI();
        }

        private RolledSavingThrowUI _savingThrowUI;

        private int _currentTargetIndex;
        private SaveTarget[] _targets;
        private int[] _targetACs;
        private List<AttackRollSelector> _rollSelectors = new List<AttackRollSelector>();
        private List<RolledAttack> _rolledAttacks = new List<RolledAttack>();
        private Dictionary<DamageTypes, DamageRollReadout> _damageTypeDict = new Dictionary<DamageTypes, DamageRollReadout>();
        private Dictionary<ConditionAndOutcome, ConditionReadout> _conditionDict = new Dictionary<ConditionAndOutcome, ConditionReadout>();
        private Dictionary<RolledAttack, AttackReadout> _attackReadoutDict = new Dictionary<RolledAttack, AttackReadout>();
        private Dictionary<Action, ActionReadout> _actionReadoutDict = new Dictionary<Action, ActionReadout>();

        [Header("Info")]
        [SerializeField]
        private TextMeshProUGUI _targetReadout;
        [Header("Selectors")]
        [SerializeField]
        private RectTransform _attackRollParent;
        [SerializeField, FormerlySerializedAs("_attackRollPrefab")]
        private GameObject _attackRollSelectorPrefab;
        [SerializeField, FormerlySerializedAs("_savingThrowPrefab")]
        private GameObject _savingThrowSelectorPrefab;
        [Header("Saving Throws")]
        [SerializeField]
        private RectTransform _strengthSavingThrows;
        [SerializeField]
        private RectTransform _dexteritySavingThrows;
        [SerializeField]
        private RectTransform _constitutionSavingThrows;
        [SerializeField]
        private RectTransform _intelligenceSavingThrows;
        [SerializeField]
        private RectTransform _wisdomSavingThrows;
        [SerializeField]
        private RectTransform _charismaSavingThrows;
        [SerializeField]
        private RectTransform _grappleSavingThrows;
        [Header("Readout")]
        [SerializeField]
        private RectTransform _readoutParent;
        [SerializeField]
        private GameObject _noDamage;
        [SerializeField]
        private RectTransform _activeHeader;
        [SerializeField]
        private RectTransform _inactiveHeader;
        [Space]
        [SerializeField, FormerlySerializedAs("_damageRollPrefab")]
        private GameObject _damageRollPrefab;
        [SerializeField]
        private GameObject _conditionReadoutPrefab;
        [SerializeField, FormerlySerializedAs("_attackReadout")]
        private GameObject _attackReadoutPrefab;
        [SerializeField, FormerlySerializedAs("_savingThrowReadout")]
        private GameObject _savingThrowReadoutPrefab;
        [SerializeField, FormerlySerializedAs("_actionReadout")]
        private GameObject _actionReadoutPrefab;
        #endregion
    }
}