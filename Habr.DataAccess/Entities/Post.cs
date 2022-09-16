﻿namespace Habr.DataAccess.Entities
{
    public class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
            PostsRatings = new HashSet<PostRating>();
            Images = new HashSet<PostImage>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsPublished { get; set; }
        public double TotalRating { get; set; }

        // User
        public int UserId { get; set; }
        public User User { get; set; }

        // Comment
        public ICollection<Comment> Comments { get; set; }

        // RatingPost
        public ICollection<PostRating> PostsRatings { get; set; }

        // Images post
        public ICollection<PostImage> Images { get; set; }
    }
}
