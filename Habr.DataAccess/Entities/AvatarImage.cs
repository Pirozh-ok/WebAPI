namespace Habr.DataAccess.Entities
{
    public class AvatarImage : Image
    {
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
