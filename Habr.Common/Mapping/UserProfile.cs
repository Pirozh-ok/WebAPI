using AutoMapper;
using Habr.Common.DTOs.UserDTOs;
using Habr.DataAccess.Entities;

namespace Habr.Common.Mapping
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(u => u.Id, c => c.MapFrom(u => u.Id))
                .ForMember(u => u.Name, c => c.MapFrom(u => u.Name))
                .ForMember(u => u.Email, c => c.MapFrom(u => u.Email))
                .ForMember(u => u.RegistrationDate, c => c.MapFrom(u => u.RegistrationDate));

            CreateMap<User, UserWithCommentsDTO>()
                .ForMember(u => u.Id, c => c.MapFrom(u => u.Id))
                .ForMember(u => u.Name, c => c.MapFrom(u => u.Name))
                .ForMember(u => u.Email, c => c.MapFrom(u => u.Email))
                .ForMember(u => u.RegistrationDate, c => c.MapFrom(u => u.RegistrationDate))
                .ForMember(u => u.Comments, c => c.MapFrom(u => u.Comments));

            CreateMap<User, UserWithPostsDTO>()
                .ForMember(u => u.Id, c => c.MapFrom(u => u.Id))
                .ForMember(u => u.Name, c => c.MapFrom(u => u.Name))
                .ForMember(u => u.Email, c => c.MapFrom(u => u.Email))
                .ForMember(u => u.RegistrationDate, c => c.MapFrom(u => u.RegistrationDate))
                .ForMember(u => u.Posts, c => c.MapFrom(u => u.Posts));

            CreateMap<User, IdentityDTO>()
                .ForMember(u => u.Id, c => c.MapFrom(u => u.Id))
                .ForMember(u => u.Name, c => c.MapFrom(u => u.Name))
                .ForMember(u => u.Email, c => c.MapFrom(u => u.Email))
                .ForMember(u => u.RegistrationDate, c => c.MapFrom(u => u.RegistrationDate))
                .ForMember(u => u.RefreshToken, c => c.MapFrom(u => u.RefreshToken))
                .ForMember(u => u.RefreshTokenExpirationDate, c => c.MapFrom(u => u.RefreshTokenExpirationDate))
                .ForMember(u => u.Role, c => c.MapFrom(u => u.Role));

            CreateMap<User, ShortUserDTO>()
               .ForMember(u => u.Name, c => c.MapFrom(u => u.Name))
               .ForMember(u => u.Email, c => c.MapFrom(u => u.Email));
        }
    }
}
