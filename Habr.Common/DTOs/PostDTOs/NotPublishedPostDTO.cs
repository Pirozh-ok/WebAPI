using Habr.Common.DTOs.ImageDTOs;

namespace Habr.Common.DTOs
{
    public class NotPublishedPostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public List<ImagePostDTO> Images { get; set; }
    }
}
