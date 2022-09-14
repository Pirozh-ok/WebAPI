namespace Habr.DataAccess.Entities
{
    public class AvatarImage
    {
        public int Id { get; set; }
        public string PathImage { get; set; }
        public DateTime LoadDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

