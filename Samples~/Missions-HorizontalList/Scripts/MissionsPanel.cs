using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mahas.ListView.Samples
{
    public class MissionsPanel : MonoBehaviour
    {
        [SerializeField] private ListView _listView;
        [SerializeField] private AnimationCurve _scrollCurve;
        
        private readonly List<BaseMissionData> _data = new();

        private void Start()
        {
            InitializeData();
            _listView.Listeners.OnCreate.AddListener(OnNewCardCreated);
            _listView.SetupData(_data);
        }

        private void OnNewCardCreated(ListViewElement listElement)
        {
            if (listElement.TryGetCard(out BaseListCard missionCard))
            {
                if (missionCard is IMissionCardProvider provider)
                {
                    provider.SetupDependencies(this);
                }
            }
        }

        private void InitializeData()
        {
            int unlockTier = 26;
            int bossMissionStep = 4;

            int bossIndex = 0;
            
            for (int i = 0; i < 130; i++)
            {
                BaseMissionData missionViewData;
                bool isUnlock = i < unlockTier;
                bool isComplete = i < unlockTier - 1;
                int tier = i + 1;
                
                if (tier % bossMissionStep == 0)
                {
                    bossIndex++;
                    int countStars = isComplete ? Random.Range(1, 4) : 0;
                    missionViewData = new BossMissionUiViewData(
                        tier, 
                        isUnlock, 
                        isComplete, 
                        countStars, 
                        bossIndex, 
                        bossIndex + 1);
                }
                else
                {
                    missionViewData = new NormalMissionUiViewData(tier, isUnlock, isComplete);
                }
                _data.Add(missionViewData);
            }
        }

        public void ScrollToActiveMission()
        {
            var nextMissionToPlay = _data.FirstOrDefault(x => x.CanPlay);
            if (nextMissionToPlay == null)
            {
                Debug.LogError("Can't find next mission to play!");
                return;
            }
            
            _listView.Manipulator
                .ScrollTo(nextMissionToPlay, 0.3f)
                .SetAlignment(AlignmentType.Center)
                .SetAnimationCurve(_scrollCurve)
                .OnComplete(OnScrollComplete)
                .Play();
            
            void OnScrollComplete()
            {
                if (_listView.Viewport.TryGetDataInstance(nextMissionToPlay, out var instance))
                {
                    if (instance is IMissionCardAnimator missionCard)
                    {
                        missionCard.PlayBounceAnimation();
                        return;
                    }
                }
                
                Debug.LogError("Can't find mission card instance to play bounce animation!");
            }
        }

    }
}