﻿using Habr.BusinessLogic;
using Habr.Common.AutoMappers;
using Habr.Common.Exceptions;
using Habr.Common.Mapping;
using Habr.DataAccess;
using Habr.Presentation.Extensions;
using Hangfire;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
var logger = LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
builder.Host.UseNLog();
//builder.WebHost.UseWebRoot(builder.Configuration.GetSection("Content").GetSection("PathContent").ToString()); 

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
builder.Services.AddFileManager();

builder.Services.AddVersioning();
builder.Services.AddHangfireServer(builder.Configuration);

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ExceptionFilter));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseStaticFiles();

app.UseApiVersioning();
app.UseSwaggerWithVersioning();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.AddHangfireDashboard();
app.StartJobs();

app.Run();
