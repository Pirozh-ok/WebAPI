using Habr.BusinessLogic;
using Habr.Common.AutoMappers;
using Habr.Common.Mapping;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Habr.Presentation.Extensions;
using Habr.Presentation.Filters;
using Hangfire;
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
builder.Services.AddAutoMapper(typeof(PostProfile), typeof(CommentProfile), typeof(UserProfile), typeof(AvatarImage));
builder.Services.AddFileManager();

builder.Services.AddVersioning();
builder.Services.AddHangfireServer(builder.Configuration);

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ExceptionFilter));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseAuthentication();

app.UseApiVersioning();
app.UseSwaggerWithVersioning();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.AddHangfireDashboard();
app.StartJobs();

app.Run();
