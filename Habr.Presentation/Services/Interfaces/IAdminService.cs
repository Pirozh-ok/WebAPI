using Habr.Common.DTOs.UserDTOs;

namespace Habr.Presentation.Services.Interfaces
{
    public interface IAdminService
    {
        Task RegisterAdminAsync(string name, string email, string password);
    }
}
