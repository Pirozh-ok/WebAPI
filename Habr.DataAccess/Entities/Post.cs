using Habr.DataAccess.Entities;

namespace Habr.DataAccess
{
    public class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsPublished { get; set; }

        // User
        public int UserId { get; set; }
        public User User { get; set; }

        // Comment
        public ICollection<Comment> Comments { get; set; }
    }
}
