using AutoMapper;
using Habr.Common.DTOs;
using Habr.DataAccess.Entities;

namespace Habr.Common.Mapping
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDTO>()
                .ForMember(p => p.Id, c => c.MapFrom(p => p.Id))
                .ForMember(p => p.AuthorName, c => c.MapFrom(p => p.User.Name))
                .ForMember(p => p.Text, c => c.MapFrom(p => p.Text))
                .ForMember(p => p.Comments, c => c.MapFrom(p => p.SubComments));
        }
    }
}
