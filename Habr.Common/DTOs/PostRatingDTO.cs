using Habr.Common.DTOs.UserDTOs;

namespace Habr.Common.DTOs
{
    public class PostRatingDTO
    {
        public int Value { get; set; }
        public DateTime DateModified { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}
