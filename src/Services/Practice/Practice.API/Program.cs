using MongoDB.Driver;
using Practice.Application.Interfaces;
using Practice.Application.Services;
using Practice.Infrastructure.ExternalServices;
using Practice.Infrastructure.Repositories;
using Practice.Infrastructure.Settings;
using Practice.Infrastructure.Grpc;
using Shared.Core.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["MongoDbSettings:ConnectionString"]));

builder.Services.AddGrpcClient<LexicalVault.LexicalVaultClient>(o =>
    o.Address = new Uri(builder.Configuration["KnowledgeBase:GrpcUrl"]!));

builder.Services.AddHttpClient<ILlmService, GroqService>();

// Repositories
builder.Services.AddScoped<IGeneratedPassageRepository, GeneratedPassageRepository>();
builder.Services.AddScoped<ISpeakingRepository, SpeakingRepository>();

// External clients
builder.Services.AddScoped<ILexicalVaultClient, LexicalVaultGrpcClient>();

// Services
builder.Services.AddScoped<IPassageService, PassageService>();
builder.Services.AddScoped<ISpeakingService, SpeakingService>();

builder.Services.AddIeltsJwtAuth(builder.Configuration);

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
