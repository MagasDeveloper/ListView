namespace Mahas.ListView.Samples
{
    public class RatingUserData : IListViewData, IHaveMessageForGizmo
    {
        public readonly UserData User;
        public readonly int Rating;
        public int Position { get; private set; }

        public RatingUserData(UserData user, int rating)
        {
            Rating = rating;
            User = user;
        }

        public void SetPosition(int position)
        {
            Position = position;
        }

        public string GetMessage()
        {
            return $"{Position}. {User.Nickname} - {Rating:N0}";
        }
    }
}