using MongoDB.Driver;
using Review.Application.Interfaces;
using Review.Application.Services;
using Review.Infrastructure.Repositories;
using Review.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["MongoDbSettings:ConnectionString"]));

builder.Services.AddScoped<IReviewCardRepository, ReviewCardRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
