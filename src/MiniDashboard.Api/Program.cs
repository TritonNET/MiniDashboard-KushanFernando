using MiniDashboard.Api.Utils;
using MiniDashboard.Common;
using MiniDashboard.DataAccess;
using MiniDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductStore, ProductStore>();
builder.Services.AddScoped<MiniDashboard.Common.ILogger, ConsoleLogger>();
builder.Services.AddScoped<IConfigProvider, APIServerConfigProvider>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
