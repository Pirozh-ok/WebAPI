﻿using Habr.DataAccess.Entities;

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

        //Post - User
        public int UserId { get; set; }
        public User User { get; set; }

        //Post - Comment
        public ICollection<Comment> Comments { get; set; }
    }
}
