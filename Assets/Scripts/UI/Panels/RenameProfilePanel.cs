using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SummonsTracker.UI
{
    public class RenameProfilePanel : Panel
    {
        public void Init(LoadProfilePanel loadProfile)
        {
            _loadProfile = loadProfile;

            _input.text = "New Summoner";
        }

        public override void Open()
        {
            base.Open();

            EventSystem.current.SetSelectedGameObject(_input.gameObject);
        }

        public void AcceptButton()
        {
            _loadProfile.OnRenameNewProfile(_input.text);
        }

        private LoadProfilePanel _loadProfile;


        [SerializeField]
        private TMP_InputField _input;
    }
}