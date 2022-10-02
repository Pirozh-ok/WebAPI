using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Habr.Common.DTOs.UserDTOs
{
    public class UserWithCommentsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public IFormFile Avatar { get; set; }
        public List<CommentDTO> Comments { get; set; }
    }
}
