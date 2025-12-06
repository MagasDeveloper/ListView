using UnityEngine;
using TMPro;

namespace Mahas.ListView.Samples
{
    public class NormalMissionUiCard : BaseMissionCard<NormalMissionUiViewData>
    {
        [SerializeField] private TextMeshProUGUI _levelTierText;
        [SerializeField] private TextMeshProUGUI _completeText;
        
        private MissionsPanel _parent;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            
            //Draw tier
            _levelTierText.text = $"Mission - {Data.Tier}";
            
            //Complete state
            _completeText.gameObject.SetActive(Data.IsComplete);
        }
        
    }
}