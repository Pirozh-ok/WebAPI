using Microsoft.AspNetCore.Http;

namespace Habr.Common
{
    public interface IFileManager
    {
        Task<string> SaveFile(IFormFile file, int userId);
        string LoadFile(string filePath); 
    }
}
