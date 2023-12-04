using TMPro;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class Readout : MonoBehaviour
    {
        public bool IsReadoutActive;
        public int AttackIndex;
        public TextMeshProUGUI TitleText => _titleText;
        public TextMeshProUGUI NoteText => _noteText;
        public CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup== null)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }
        public void OnSetActive()
        {
            CanvasGroup.interactable = true;
            CanvasGroup.alpha = 1f;
        }
        public void OnSetInactive()
        {
            CanvasGroup.interactable = true;
            CanvasGroup.alpha = 0.4f;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private CanvasGroup _canvasGroup;

        [SerializeField]
        private TextMeshProUGUI _titleText;
        [SerializeField]
        private TextMeshProUGUI _noteText;
    }
}