
namespace Mahas.ListView.Samples
{
    public class NormalMissionUiViewData : BaseMissionData, IHaveMessageForGizmo
    {
        
        public NormalMissionUiViewData(int tier, bool isUnlocked, bool isComplete) : base(tier, isUnlocked, isComplete)
        {
        }

        public string GetMessage()
        {
            return $"Level: {Tier}";
        }
    }
} 