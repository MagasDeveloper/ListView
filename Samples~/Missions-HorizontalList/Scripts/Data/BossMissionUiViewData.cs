
namespace Mahas.ListView.Samples
{
    public class BossMissionUiViewData : BaseMissionData
    {
        public readonly int CountStars;

        public readonly int RequiredHammerLevel;
        public readonly int RequiredDaggerLevel;

        public BossMissionUiViewData(int tier,
            bool isUnlocked,
            bool isComplete,
            int countStars,
            int requiredHammerLevel,
            int requiredDaggerLevel) : base(tier, isUnlocked, isComplete)
        {
            CountStars = countStars;
            RequiredHammerLevel = requiredHammerLevel;
            RequiredDaggerLevel = requiredDaggerLevel;
        }
    }
}