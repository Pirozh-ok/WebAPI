using Microsoft.AspNetCore.Http;

namespace Habr.Common.DTOs.UserDTOs
{
    public class UserWithPostsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public List<PostDTO> Posts { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
