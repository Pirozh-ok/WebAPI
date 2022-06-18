using AutoMapper;
using Habr.BusinessLogic.Services.Implementations;
using Habr.Common.AutoMappers;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using NLog;
using NUnit.Framework;

namespace Habr.BusinessLogic.Tests.Services
{
    public class PostServiceTest
    {
        private static DbContextOptions<DataContext> _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "HabrDbTest")
            .Options;

        private DataContext _context;
        private IMapper _mapper;
        private ILogger _logger; 

        [OneTimeSetUp]
        public async void Setup()
        {
            _context = new DataContext(_dbContextOptions);
            await _context.Database.EnsureCreatedAsync();

            _mapper = new Mapper( new MapperConfiguration(
                options =>
                {
                    options.AddProfile(typeof(PostProfile));
                }));

            var loggerMock = new Mock<ILogger>();
            loggerMock
            SeedDatabase();
        }

        [OneTimeTearDown]
        public async void CleanUp()
        {
            await _context.Database.EnsureDeletedAsync(); 
        }

        [Fact]
        public async void CreatePost_CorrectData_Success()
        {
            // Avarage
            AutoMapper.IMapper mapper = new AutoMapper.IMapper();
            var postService = new PostService(_context, null, null);
            var user = _context.Users.First();
            var title = "Post4";
            var text = "Text4";

            // Act
            await postService.CreatePostAsync(title, text, user.Id, false);

            // Assert
            Xunit.Assert.NotNull(await _context.Posts.SingleOrDefaultAsync(p => p.Title == title));
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
                    //Created = DateTime.Now,
                    //Updated = DateTime.Now
                },

                new Post()
                {
                    Title = "Post2",
                    Text = "Text2",
                    UserId = user2.Id,
                    IsPublished = false,
                    //Created = DateTime.Now,
                    //Updated = DateTime.Now
                },

                new Post()
                {
                    Title = "Post3",
                    Text = "Text3",
                    User = user2,
                    IsPublished = false,
                    //Created = DateTime.Now,
                    //Updated = DateTime.Now
                }
            });

            await _context.SaveChangesAsync();
        }
    }
}
