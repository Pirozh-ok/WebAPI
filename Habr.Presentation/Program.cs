using Habr.BusinessLogic;
using Habr.Common.AutoMappers;
using Habr.Common.Logging;
using Habr.Common.Mapping;
using Habr.DataAccess;
using NLog;

var builder = WebApplication.CreateBuilder(args);
LogManager.LoadConfiguration(@"C:\Users\ivan-\source\repos\Habr\Habr.Common\Logging\NLog.config");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddServices();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(PostProfile), typeof(CommentProfile), typeof(UserProfile));
builder.Services.AddSwaggerDocument(config =>
{
    config.PostProcess = document =>
    {
        document.Info.Version = "v1";
        document.Info.Title = "Habr API";
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOpenApi();
app.UseSwaggerUi3();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
