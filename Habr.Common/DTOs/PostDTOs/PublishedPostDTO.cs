using Habr.Common.DTOs.ImageDTOs;

namespace Habr.Common.DTOs
{
    public class PublishedPostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime PublicationDate { get; set; }
        public double Rating { get; set; }
        public List<CommentDTO> Comments { get; set; }
        public List<ImagePostDTO> Images { get; set; }
    }
}
