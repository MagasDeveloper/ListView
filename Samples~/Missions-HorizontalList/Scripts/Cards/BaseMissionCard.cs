using UnityEngine;

namespace Mahas.ListView.Samples
{
    public abstract class BaseMissionCard<TData> :
        ListViewCard<TData>,
        IMissionCardAnimator,
        IMissionCardProvider
        where TData : BaseMissionData
    {
        [SerializeField] private UiButton _playButton;

        private MissionsPanel _parentPanel;

        protected override void OnCreate()
        {
            _playButton.SetSuccessClickAction(StartMission);
            _playButton.SetLockClickAction(() => { _parentPanel.ScrollToActiveMission(); });
        }

        public void SetupDependencies(MissionsPanel parentPanel)
        {
            _parentPanel = parentPanel;
        }

        protected override void OnSpawn()
        {
            //Play button
            _playButton.SetLockState(!Data.IsUnlocked);
            _playButton.gameObject.SetActive(!Data.IsComplete);
        }

        private void StartMission()
        {
            //Process start mission:
            //Show screen fade?
            //Load mission configs?
            //Wait for server response?
            //Something else? why not :D
        }

        public void PlayBounceAnimation()
        {
            //Play some animation with DOTween or Animator
            Debug.Log("Play bounce animation on mission card");
        }
    }
}