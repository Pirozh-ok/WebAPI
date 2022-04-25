using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Services.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetPosts();
        IEnumerable<Post> GetPostsByUser(int userId);
        IEnumerable<Post> GetPostsWithComment();
        Post GetPostById(int id);
        void CreatePost(string title, string text, int userId);
        void UpdatePost(Post post);
        void DeletePost(int postId);
    }
}
