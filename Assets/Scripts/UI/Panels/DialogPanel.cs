using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    public class DialogPanel : Panel
    {
        public static DialogPanel ShowDialogPanel(string header, string body, Action accept = null)
        {
            if (_instance == null)
            {
                _instance = UnityEngine.Object.FindObjectOfType<DialogPanel>(true);
            }
            _instance.SetDialogData(header, body, accept);
            _instance.Open();
            return _instance;
        }

        public void SetDialogData(string header, string body, Action accept = null)
        {
            _header.text = header;
            _body.text = body;
            _cancelButton.gameObject.SetActive(accept != null);
            _onAccept = accept;
        }

        public void AcceptButton()
        {
            _onAccept?.Invoke();
            Close();
        }

        public void CloseButton()
        {
            Close();
        }

        protected override void Awake()
        {
            base.Awake();
            _instance = this;
        }

        private static DialogPanel _instance;

        private Action _onAccept;

        [SerializeField]
        private TextMeshProUGUI _header;
        [SerializeField]
        private TextMeshProUGUI _body;
        [SerializeField]
        private Button _acceptButton;
        [SerializeField]
        private Button _cancelButton;
    }
}