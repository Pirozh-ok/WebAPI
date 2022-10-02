﻿using Habr.Common.DTOs.ImageDTOs;
using Habr.Common.DTOs.UserDTOs;

namespace Habr.Common.DTOs.PostDTOs
{
    public class PublishedPostDTOv2
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public ShortUserDTO Author { get; set; }
        public double Rating { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<CommentDTO> Comments { get; set; }
        public List<ImagePostDTO> Images { get; set; }
    }
}
