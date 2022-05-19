using Habr.BusinessLogic;
using Habr.Common.AutoMappers;
using Habr.Common.Mapping;
using Habr.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddServices();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(PostProfile), typeof(CommentProfile), typeof(UserProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
