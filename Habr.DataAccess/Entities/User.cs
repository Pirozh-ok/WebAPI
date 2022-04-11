using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Habr.DataAccess.Entities
{
    public class User
    {
        public User()
        {
            Posts = new HashSet<Post>();
            Comments = new HashSet<Comment>(); 
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime RegistrDate { get; set; }

        //User - Post
        public ICollection<Post> Posts { get; set; }

        // User - Comment
        public ICollection<Comment> Comments { get; set; }
    }
}
