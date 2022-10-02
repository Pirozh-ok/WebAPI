namespace Habr.DataAccess.Entities
{
    public class User
    {
        public User()
        {
            Posts = new HashSet<Post>();
            Comments = new HashSet<Comment>();
            PostsRatings = new HashSet<PostRating>();
            Subscriptions = new HashSet<UserSubscriptions>(); 
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationDate { get; set; }
        public Roles Role { get; set; }
        public AvatarImage AvatarImage { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }

        // Post
        public ICollection<Post> Posts { get; set; }

        // Comment
        public ICollection<Comment> Comments { get; set; }

        // PostRating
        public ICollection<PostRating> PostsRatings { get; set; }

        // Subscriptions
        public ICollection<UserSubscriptions> Subscriptions { get; set; }
    }
}
