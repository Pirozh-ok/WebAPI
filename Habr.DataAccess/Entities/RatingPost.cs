namespace Habr.DataAccess.Entities
{
    public class RatingPost
    {
        public int Id;
        public int UserId;
        public int PostId;
        public int Value;
        public DateTime DateLastModified;
        public User User;
        public Post Post;
    }
}
