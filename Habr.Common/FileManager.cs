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
        private string _imagePathtoSave;
        private string _imagePathToDB; 

        public FileManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _imagePathtoSave = configuration["Content:PathImagesToSave"];
            _imagePathToDB = configuration["Content:PathImagesToGet"]; 
        }

        public async Task<string> LoadFile(string filePath)
        {
            var loadPath = Path.Combine(_imagePathtoSave, filePath);

            if(!File.Exists(loadPath))
            {
                throw new BusinessException(UserExceptionMessageResource.ErrorGetAvatar); 
            }

            using var fs = new FileStream(loadPath, FileMode.Open, FileAccess.Read);
            return await fs.FileStreamToBase64(); 
        }

        public async Task<string> SaveFile(IFormFile image, int userId)
        {

            if(!Directory.Exists(_imagePathtoSave))
            {
                Directory.CreateDirectory(_imagePathtoSave);
            }

            var mime = image.FileName
                            .Substring(image.FileName.LastIndexOf('.'));

            var fileName = $"user{userId}_img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{mime}";
            var savePath = Path.Combine(_imagePathtoSave, fileName);

            using var fs = new FileStream(savePath, FileMode.Create);
            await image.CopyToAsync(fs);

            return Path.Combine(_imagePathToDB, fileName); 
        }
    }
}
