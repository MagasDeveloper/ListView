
namespace Mahas.ListView.Samples
{
    public abstract class BaseMissionData : IListViewData
    {
        public readonly int Tier;
        public readonly bool IsUnlocked;
        public readonly bool IsComplete;

        public bool CanPlay => IsUnlocked && !IsComplete;

        protected BaseMissionData(int tier, bool isUnlocked, bool isComplete)
        {
            Tier = tier;
            IsUnlocked = isUnlocked;
            IsComplete = isComplete;
        }
    }
}