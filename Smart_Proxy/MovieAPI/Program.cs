using Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MovieAPI.Repositories;
using MovieAPI.Services;
using MovieAPI.Settings;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.Configure<SyncServiceSettings>(builder.Configuration.GetSection("SyncServiceSettings"));

builder.Services.AddSingleton<IMongoDbSettings>(provider => provider.GetRequiredService<IOptions<MongoDbSettings>>().Value);
builder.Services.AddSingleton<ISyncServiceSettings>(provider => provider.GetRequiredService<IOptions<SyncServiceSettings>>().Value);

builder.Services.AddScoped<IMongoRepository<Movie>, MongoRepository<Movie>>();
builder.Services.AddTransient<ISyncService<Movie>, SyncService<Movie>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
