using Habr.BusinessLogic;
using Habr.Common.AutoMappers;
using Habr.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
