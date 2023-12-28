using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SummonsTracker.UI
{
    public class CreateTargetPanel : Panel
    {
        public void Init(Action<string, bool> onClose)
        {
            _input.text = "New Target";
            _onClose = onClose;
        }

        public override void Open()
        {
            base.Open();

            EventSystem.current.SetSelectedGameObject(_input.gameObject);
        }

        public void AcceptButton()
        {
            _onClose?.Invoke(_input.text, true);
            Close();
        }

        public void CancelButton()
        {
            _onClose?.Invoke(string.Empty, false);
            Close();
        }

        private Action<string, bool> _onClose;


        [SerializeField]
        private TMP_InputField _input;
    }
}