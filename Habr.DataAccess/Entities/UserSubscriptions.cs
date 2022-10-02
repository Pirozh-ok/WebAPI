namespace Habr.DataAccess.Entities
{
    public class UserSubscriptions
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public int SubsUserId { get; set; }
        public DateTime DateSubscribe { get; set; }
    }
}
