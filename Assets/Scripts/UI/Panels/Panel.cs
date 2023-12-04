using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummonsTracker.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Panel : MonoBehaviour
    {
        public int Layer => _layer;
        public int Priority => _priority;

        public virtual void Open()
        {
            Debug.Log($"Open: {name}");
            var panels = _activePanels.ToArray();
            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i] == this || panels[i]._closing)
                {
                    continue;
                }
                if (panels[i]._layer >= this._layer)
                {
                    panels[i].Close(false);
                }
            }
            gameObject.SetActive(true);
            GetCanvasGroup().interactable = true;
            if (_lastOpenedPanel != null && _lastOpenedPanel != this && !_lastOpenedPanel._closing)
            {
                _openedFrom = _lastOpenedPanel;
            }
            _lastOpenedPanel = this;
        }

        public virtual void Close()
        {
            Close(true);
        }

        public virtual void Close(bool autoOpen)
        {
            _closing = true;
            gameObject.SetActive(false);
            GetCanvasGroup().interactable = false;
            if (autoOpen)
            {
                if (_openedFrom != null)
                {
                    _openedFrom.Open();
                    _openedFrom = null;
                }
                else
                {
                    var index = -1;
                    var maxLayer = _layer;
                    var panels = _activePanels.ToArray();
                    for (int i = 0; i < panels.Length; i++)
                    {
                        if (panels[i] == this)
                        {
                            continue;
                        }
                        if (panels[i]._layer > maxLayer)
                        {
                            index = i;
                            maxLayer = panels[i]._layer;
                        }
                    }
                    if (index >= 0)
                    {
                        panels[index].Open();
                    }
                }
            }
            _closing = false;
        }

        #region Unity Messages
        protected virtual void Awake()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
            _activePanels.Add(this);
        }

        protected virtual void OnDestroy()
        {
            _activePanels.Remove(this);
        }
        #endregion

        #region Private
        private CanvasGroup GetCanvasGroup()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = this.GetComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }

        private Panel _openedFrom;
        private static Panel _lastOpenedPanel;
        private static List<Panel> _activePanels = new List<Panel>();
        private CanvasGroup _canvasGroup;

        private bool _closing;

        [SerializeField, FormerlySerializedAs("_Layer")]
        private int _layer;
        [SerializeField]
        private int _priority;
        #endregion
    }
}