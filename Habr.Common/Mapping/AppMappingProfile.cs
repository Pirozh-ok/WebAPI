using AutoMapper;
using Habr.Common.DTOs;
using Habr.DataAccess;
using Habr.DataAccess.Entities;

namespace Habr.Common.AutoMappers
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<Post, PostDTO>()
                .ForMember(p => p.Title, c => c.MapFrom(p => p.Title))
                .ForMember(p => p.EmailAuthor, c => c.MapFrom(p => p.User.Email))
                .ForMember(p => p.CreateDate, c => c.MapFrom(p => p.Created));

            CreateMap<Post, PublishedPostDTO>()
                .ForMember(p => p.Title, c => c.MapFrom(p => p.Title))
                .ForMember(p => p.Text, c => c.MapFrom(p => p.Text))
                .ForMember(p => p.AuthorEmail, c => c.MapFrom(p => p.User.Email))
                .ForMember(p => p.PublicationDate, c => c.MapFrom(p => p.Updated))
                .ForMember(p => p.Comments, c => c.MapFrom(p => p.Comments));

            CreateMap<Post, NotPublishedPostDTO>()
                .ForMember(p => p.Title, c => c.MapFrom(p => p.Title))
                .ForMember(p => p.Created, c => c.MapFrom(p => p.Created))
                .ForMember(p => p.Updated, c => c.MapFrom(p => p.Updated));

            CreateMap<Comment, CommentDTO>()
                .ForMember(p => p.AuthorName, c => c.MapFrom(p => p.User.Name))
                .ForMember(p => p.Text, c => c.MapFrom(p => p.Text))
                .ForMember(p => p.Comments, c => c.MapFrom(p => p.SubComments));
        }
    }
}
