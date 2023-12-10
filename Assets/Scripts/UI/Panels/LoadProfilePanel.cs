using SummonsTracker.Save;
using System;
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
                for (int i = 0; i < SaveManager.Instance.Current.Profiles.Length; i++)
                {
                    Profile profile = SaveManager.Instance.Current.Profiles[i];
                    var instGO = GetGameObject(profile);
                    instGO.transform.SetSiblingIndex(_noProfileText.transform.GetSiblingIndex());

                    var entry = instGO.GetComponent<SaveProfileEntry>();
                    entry.Initialise(this, profile, i);

                    _profileButtons.Add(entry);
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
            base.Close();

            for (int i = _profileButtons.Count - 1; i >= 0; i--)
            {
                Destroy(_profileButtons[i].gameObject);
            }
            _profileButtons.Clear();
        }

        public void OnRenameNewProfile(string text)
        {
            var p = SaveManager.Instance.CreateNewProfile(text);
            SaveManager.Instance.Save();
            _mainPanel.OnLoadProfile(p);
            _mainPanel.Open();
        }

        public void LoadProfile(int index)
        {
            _mainPanel.OnLoadProfile(SaveManager.Instance.LoadProfile(index));
            _mainPanel.Open();
        }

        public void DeleteProfile(int index)
        {
            DialogPanel.ShowDialogPanel("Delete", $"Delete profile\n\"{SaveManager.Instance.Current.Profiles[index].Name}\"", delegate
            {
                SaveManager.Instance.DeleteProfile(index);
            });
        }

        public void CreateNewProfileButton()
        {
            _renameProfilePanel.Init(this);
            _renameProfilePanel.Open();
        }

        protected override void OnClose()
        {
        }

        private GameObject GetGameObject(Profile profile)
        {
            foreach (var p in _profileButtons)
            {
                if (profile == p.Profile)
                {
                    return p.gameObject;
                }
            }
            return GameObject.Instantiate(_profilePrefab, _parent);
        }

        [Header("References")]
        [SerializeField]
        private MainPanel _mainPanel;
        [SerializeField]
        private RenameProfilePanel _renameProfilePanel;
        [SerializeField]
        private RectTransform _parent;
        [SerializeField]
        private GameObject _noProfileText;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject _profilePrefab;

        private List<SaveProfileEntry> _profileButtons = new List<SaveProfileEntry>();
    }
}