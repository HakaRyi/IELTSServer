using KnowledgeBase.Application.Interfaces;
using KnowledgeBase.Application.Services;
using KnowledgeBase.Domain.Interfaces;
using KnowledgeBase.Infrastructure.Repositories;
using KnowledgeBase.Infrastructure.Settings;
using KnowledgeBase.Infrastructure.ExternalServices;
using KnowledgeBase.API.GrpcServices;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
//------------------------------------------------------------------------------
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
    return new MongoClient(settings?.ConnectionString);
});
#region Repositories
builder.Services.AddScoped<ILexicalItemRepository, LexicalItemRepository>();
builder.Services.AddScoped<IGeneratedPassageRepository, GeneratedPassageRepository>();
#endregion

#region Services
builder.Services.AddScoped<ILexicalItemService, LexicalItemService>();
builder.Services.AddScoped<IPassageService, PassageService>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
#endregion

#region Grpc
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
#endregion
//------------------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<LexicalGrpcService>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
