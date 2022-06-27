using Habr.BusinessLogic;
using Habr.Common.AutoMappers;
using Habr.Common.Exceptions;
using Habr.Common.Mapping;
using Habr.DataAccess;
using Habr.Presentation.Extensions;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
var logger = LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
builder.Host.UseNLog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddJwtAuthorization(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddAdminService();
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

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ExceptionFilter));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseOpenApi();
app.UseSwaggerUi3();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
