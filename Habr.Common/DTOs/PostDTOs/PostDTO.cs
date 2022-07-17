﻿namespace Habr.Common.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string EmailAuthor { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsPublished { get; set; }
        public double Rating { get; set; }
    }
}
