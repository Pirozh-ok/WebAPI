﻿using AutoMapper;
using Habr.Common.DTOs;
using Habr.Common.DTOs.PostDTOs;
using Habr.DataAccess.Entities;

namespace Habr.Common.AutoMappers
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostDTO>()
                .ForMember(p => p.Id, c => c.MapFrom(p => p.Id))
                .ForMember(p => p.Title, c => c.MapFrom(p => p.Title))
                .ForMember(p => p.EmailAuthor, c => c.MapFrom(p => p.User.Email))
                .ForMember(p => p.CreateDate, c => c.MapFrom(p => p.Created))
                .ForMember(p => p.IsPublished, c => c.MapFrom(p => p.IsPublished))
                .ForMember(p => p.Rating, c => c.MapFrom(p => p.TotalRating))
                .ForMember(p => p.Images, c => c.MapFrom(p => p.Images)); 
                

            CreateMap<Post, PublishedPostDTO>()
                .ForMember(p => p.Title, c => c.MapFrom(p => p.Title))
                .ForMember(p => p.Text, c => c.MapFrom(p => p.Text))
                .ForMember(p => p.AuthorEmail, c => c.MapFrom(p => p.User.Email))
                .ForMember(p => p.PublicationDate, c => c.MapFrom(p => p.Updated))
                .ForMember(p => p.Comments, c => c.MapFrom(p => p.Comments))
                .ForMember(p => p.Rating, c => c.MapFrom(p => p.TotalRating))
                .ForMember(p => p.Images, c => c.MapFrom(p => p.Images));

            CreateMap<Post, PublishedPostDTOv2>()
                .ForMember(p => p.Title, c => c.MapFrom(p => p.Title))
                .ForMember(p => p.Text, c => c.MapFrom(p => p.Text))
                .ForMember(p => p.Author, c => c.MapFrom(p => p.User))
                .ForMember(p => p.PublicationDate, c => c.MapFrom(p => p.Updated))
                .ForMember(p => p.Comments, c => c.MapFrom(p => p.Comments))
                .ForMember(p => p.Rating, c => c.MapFrom(p => p.TotalRating))
                .ForMember(p => p.Images, c => c.MapFrom(p => p.Images));

            CreateMap<Post, NotPublishedPostDTO>()
                .ForMember(p => p.Title, c => c.MapFrom(p => p.Title))
                .ForMember(p => p.Created, c => c.MapFrom(p => p.Created))
                .ForMember(p => p.Updated, c => c.MapFrom(p => p.Updated))
                .ForMember(p => p.Images, c => c.MapFrom(p => p.Images));

            CreateMap<PostRating, PostRatingDTO>()
                .ForMember(p => p.Value, c => c.MapFrom(p => p.Value))
                .ForMember(p => p.DateModified, c => c.MapFrom(p => p.DateLastModified))
                .ForMember(p => p.UserId, c => c.MapFrom(p => p.UserId))
                .ForMember(p => p.UserName, c => c.MapFrom(p => p.User.Name))
                .ForMember(p => p.UserEmail, c => c.MapFrom(p => p.User.Email));
        }
    }
}
