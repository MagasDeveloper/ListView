using UnityEngine;
using TMPro;

namespace Mahas.ListView.Samples
{
    public class BossMissionUiCard : BaseMissionCard<BossMissionUiViewData>
    {
        [Header("Main")] 
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private TextMeshProUGUI _completeText;

        [Header("Stars")] 
        [SerializeField] private GameObject[] _activeStars;

        [Header("Requirements")] 
        [SerializeField] private TextMeshProUGUI _hammerLevelText;
        [SerializeField] private TextMeshProUGUI _daggerLevelText;


        protected override void OnSpawn()
        {
            base.OnSpawn();
            _headerText.text = $"Boss Mission - {Data.Tier}";

            //Complete state
            _completeText.gameObject.SetActive(Data.IsComplete);

            //Stars
            for (int i = 0; i < _activeStars.Length; i++)
            {
                _activeStars[i].SetActive(i < Data.CountStars);
            }

            //Requirements
            _hammerLevelText.text = $"Level-{Data.RequiredHammerLevel}";
            _daggerLevelText.text = $"Level-{Data.RequiredDaggerLevel}";
        }

    }
}