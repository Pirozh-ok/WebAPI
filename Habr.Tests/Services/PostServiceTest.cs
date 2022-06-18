using System.Text;
using AutoMapper;
using Habr.BusinessLogic.Services.Implementations;
using Habr.Common.AutoMappers;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Habr.BusinessLogic.Tests.Services
{
    public class PostServiceTest
    {
        private static DbContextOptions<DataContext> _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "HabrDbTest")
            .Options;

        private DataContext _context;
        private IMapper _mapper;
        private ILogger<PostService> _logger;

        public PostServiceTest()
        {
            _context = new DataContext(_dbContextOptions);
            _context.Database.EnsureCreated();

            _mapper = new Mapper(new MapperConfiguration(
                options =>
                {
                    options.AddProfile(typeof(PostProfile));
                }));

            var loggerMock = new Mock<ILogger<PostService>>();
            _logger = loggerMock.Object;

            SeedDatabase();
        }

        [Fact]
        public async void CreatePost_CorrectData_Success()
        {
            // Avarage

            var postService = new PostService(_context, _mapper, _logger);
            var user = await _context.Users.FirstOrDefaultAsync(); 
            var title = "Post4";
            var text = "Text4";
    
            // Act

            await postService.CreatePostAsync(title, text, user.Id, false);

            // Assert

            Assert.True(await _context.Posts
                .SingleOrDefaultAsync(p => p.Title == title) is not null);
        }

        [Fact]
        public async void CreatePost_NullOrEmptyTitle_ThrowValidationException()
        {
            // Avarage

            var postService = new PostService(_context, _mapper, _logger);
            var user = await _context.Users.FirstOrDefaultAsync();
            var title = string.Empty;
            var text = "Text4";

            // Act

            Func<Task> act = () => postService.CreatePostAsync(title, text, user.Id, false);

            // Assert

            await Assert.ThrowsAsync<ValidationException>(act);
        }

        [Fact]
        public async void CreatePost_TitleOfLengthExceedsMax_ThrowValidationException()
        {
            // Avarage

            var postService = new PostService(_context, _mapper, _logger);
            var user = await _context.Users.FirstOrDefaultAsync();
            var title = new StringBuilder(300);
            title.Length = 300; 
            var text = "Text4";

            // Act

            Func<Task> act = () => postService.CreatePostAsync(title.ToString(), text, user.Id, false);

            // Assert

            await Assert.ThrowsAsync<ValidationException>(act);
        }

        [Fact]
        public async void CreatePost_NullOrEmptyText_ThrowValidationException()
        {
            // Avarage

            var postService = new PostService(_context, _mapper, _logger);
            var user = await _context.Users.FirstOrDefaultAsync();
            var title = "Post4";
            var text = string.Empty;

            // Act

            Func<Task> act = () => postService.CreatePostAsync(title, text, user.Id, false);

            // Assert

            await Assert.ThrowsAsync<ValidationException>(act);
        }

        [Fact]
        public async void CreatePost_TextOfLengthExceedsMax_ThrowValidationException()
        {
            // Avarage

            var postService = new PostService(_context, _mapper, _logger);
            var user = await _context.Users.FirstOrDefaultAsync();
            var title = "Post4";
            var text = new StringBuilder(2500);
            text.Length = 2500;

            // Act

            Func<Task> act = () => postService.CreatePostAsync(title, text.ToString(), user.Id, false);

            // Assert

            await Assert.ThrowsAsync<ValidationException>(act);
        }

        [Fact]
        public async void CreatePost_InvalidAuthorId_ThrowBadRequestException()
        {
            // Avarage

            var postService = new PostService(_context, _mapper, _logger);
            var userId = -1;
            var title = "Post4";
            var text = "Text4";

            // Act

            Func<Task> act = () => postService.CreatePostAsync(title, text, userId, false);

            // Assert

            await Assert.ThrowsAsync<BadRequestException>(act);
        }

        [Fact]
        public async void DeletePost_CorrectId_Success()
        {
            // Avarage

            var postService = new PostService(_context, _mapper, _logger);
            int postId = 1;

            // Act

            await postService.DeletePostAsync(postId);

            // Assert

            Assert.True(await _context.Posts.SingleOrDefaultAsync(p => p.Id == postId) is null);
        }

        [Fact]
        public async void DeletePost_NotExists_Success()
        {
            // Avarage

            var postService = new PostService(_context, _mapper, _logger);
            int postId = -1;

            // Act

            Func<Task> act = () => postService.DeletePostAsync(postId);;

            // Assert

            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        private async void SeedDatabase()
        {
            var user1 = new User
            {
                Name = "User1",
                Email = "Email1",
                Password = "Password1",
                RegistrationDate = DateTime.Now
            };

            var user2 = new User
            {
                Name = "User2",
                Email = "Email2",
                Password = "Password2",
                RegistrationDate = DateTime.Now
            };

            await _context.Users.AddRangeAsync(user1, user2);

            await _context.Posts.AddRangeAsync( new List<Post>
            {
                new Post()
                {
                    Title = "Post1",
                    Text = "Text1",
                    UserId = user1.Id,
                    IsPublished = true,
                },

                new Post()
                {
                    Title = "Post2",
                    Text = "Text2",
                    UserId = user2.Id,
                    IsPublished = false,
                },

                new Post()
                {
                    Title = "Post3",
                    Text = "Text3",
                    User = user2,
                    IsPublished = false,
                }
            });

            await _context.SaveChangesAsync();
            var users = _context.Users.ToList();
        }

        /*[OneTimeTearDown]
        public async void CleanUp()
        {
            await _context.Database.EnsureDeletedAsync();
        }*/
    }
}
