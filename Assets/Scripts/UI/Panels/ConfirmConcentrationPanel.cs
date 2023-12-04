using SummonsTracker.Manager;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class ConfirmConcentrationPanel : Panel
    {
        public override void Open()
        {
            base.Open();
            _summonText.text = string.Join("\n", GameManager.Instance.MainScene.ConcentrationCharacters.Select(c => c.Name).ToArray());
        }

        public void Confirm()
        {
            OnConfirm?.Invoke();
        }

        public Action OnConfirm;

        [SerializeField]
        private TextMeshProUGUI _summonText;
    }
}