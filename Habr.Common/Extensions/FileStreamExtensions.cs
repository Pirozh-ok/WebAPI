namespace Habr.Common.Extensions
{
    static public class FileStreamExtensions
    {
        static public string FileStreamToBase64(this FileStream fs)
        {
            byte[] bytes;

            using var ms = new MemoryStream();
            fs.CopyToAsync(ms);
            bytes = ms.ToArray();

            return(Convert.ToBase64String(bytes));
        }
    }
}
