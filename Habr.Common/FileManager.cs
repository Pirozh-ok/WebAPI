using Habr.Common.Exceptions;
using Habr.Common.Extensions;
using Habr.Common.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace Habr.Common
{
    public class FileManager : IFileManager
    {
        private readonly IConfiguration _configuration;
        private string imagePath; 
        public FileManager(IConfiguration configuration)
        {
            _configuration = configuration;
            imagePath = configuration["Content:PathImages"];
        }

        public async Task<string> LoadFile(string filePath)
        {
            var loadPath = Path.Combine(imagePath, filePath);

            if(!File.Exists(loadPath))
            {
                throw new BusinessException(UserExceptionMessageResource.ErrorGetAvatar); 
            }

            using var fs = new FileStream(loadPath, FileMode.Open, FileAccess.Read);
            return await fs.FileStreamToBase64(); 
        }

        public async Task<string> SaveFile(IFormFile image, int userId)
        {

            if(!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            var mime = image.FileName
                            .Substring(image.FileName.LastIndexOf('.'));

            var fileName = $"user{userId}_img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{mime}";
            var savePath = Path.Combine(imagePath, fileName);

            using var fs = new FileStream(savePath, FileMode.Create);
            await image.CopyToAsync(fs);

            return savePath; 
        }
    }
}
