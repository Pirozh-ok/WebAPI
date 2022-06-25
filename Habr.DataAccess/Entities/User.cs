using Habr.Presentation.Auth;

namespace Habr.DataAccess.Entities
{
    public class User
    {
        public User()
        {
            Posts = new HashSet<Post>();
            Comments = new HashSet<Comment>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationDate { get; set; }
        public RefreshToken RefreshToken { get; set; }

        // Post
        public ICollection<Post> Posts { get; set; }

        // Comment
        public ICollection<Comment> Comments { get; set; }
    }
}
