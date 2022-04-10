using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Habr.DataAccess.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public int ParentId { get; set; }
        public int IdPost { get; set; }
        public Post Post { get; set; }
        public int UserId { get;set; }
        public User User { get; set; } 
    }
}
