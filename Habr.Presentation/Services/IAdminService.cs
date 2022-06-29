using Habr.Common.DTOs.UserDTOs;

namespace Habr.Presentation.Services
{
    public interface IAdminService
    {
        Task RegisterAdminAsync(string name, string email, string password);
    }
}
