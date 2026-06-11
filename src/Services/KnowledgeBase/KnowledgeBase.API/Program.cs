using KnowledgeBase.Application.Interfaces;
using KnowledgeBase.Application.Services;
using KnowledgeBase.Domain.Interfaces;
using KnowledgeBase.Infrastructure.Repositories;
using KnowledgeBase.Infrastructure.Settings;
using KnowledgeBase.Infrastructure.ExternalServices;
using KnowledgeBase.API.GrpcServices;
using MongoDB.Driver;
using Shared.Core.Authentication;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Cleartext (không TLS) KHÔNG thể dùng chung HTTP/1.1 + HTTP/2 trên một cổng (thiếu ALPN).
// → Tách: 5101 cho REST (HTTP/1.1, mobile) | 5102 cho gRPC (HTTP/2 h2c, Practice gọi).
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5101, o => o.Protocols = HttpProtocols.Http1);
    options.ListenAnyIP(5102, o => o.Protocols = HttpProtocols.Http2);
});
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
#endregion

#region Services
builder.Services.AddScoped<ILexicalItemService, LexicalItemService>();
builder.Services.AddHttpClient<ILlmService, GroqService>();
#endregion

#region Grpc
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
#endregion

builder.Services.AddIeltsJwtAuth(builder.Configuration);
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
