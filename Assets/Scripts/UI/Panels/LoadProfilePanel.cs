using SummonsTracker.Save;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.UI
{
    public class LoadProfilePanel : Panel
    {
        public override void Open()
        {
            if (SaveManager.Instance.Current.Profiles.Any())
            {
                foreach (var profile in SaveManager.Instance.Current.Profiles)
                {
                    var instGO = GameObject.Instantiate(_profilePrefab, _parent);
                    instGO.transform.SetSiblingIndex(_noProfileText.transform.GetSiblingIndex());

                    _profileButtons.Add(instGO);
                }
                _noProfileText.gameObject.SetActive(false);
            }
            else
            {
                _noProfileText.gameObject.SetActive(true);
            }
            base.Open();
        }

        public override void Close()
        {
            for (int i = _profileButtons.Count - 1; i >= 0; i--)
            {
                Destroy(_profileButtons[i]);
            }
            _profileButtons.Clear();
            base.Close();
        }

        public void CreateNewProfileButton()
        {
            SaveManager.Instance.CreateNewProfile("New Summoner");
            Close();
        }

        [Header("References")]
        [SerializeField]
        private RectTransform _parent;
        [SerializeField]
        private GameObject _noProfileText;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject _profilePrefab;

        private List<GameObject> _profileButtons = new List<GameObject>();
    }
}