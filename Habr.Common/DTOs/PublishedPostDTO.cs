using Habr.DataAccess.Entities;

namespace Habr.Common.DTOs
{
    public class PublishedPostDTO
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<CommentDTO> Comments { get; set; }
    }
}
