using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Habr.DataAccess.Entities
{
    public class Comment
    {
        public Comment()
        {
            Comments = new HashSet<Comment>();
        }
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }

        //Comment - Comment
        public int ParentId { get; set; }
        public Comment ParentComment { get; set; }
        public ICollection<Comment> Comments { get; set; }

        //Comment - Post
        public int PostId { get; set; }
        public Post Post { get; set; }

        //Comment - User
        public int UserId { get;set; }
        public User User { get; set; } 
    }
}
