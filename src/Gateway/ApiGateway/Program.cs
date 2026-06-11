var builder = WebApplication.CreateBuilder(args);

// YARP reverse proxy — đọc routes/clusters từ appsettings "ReverseProxy"
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// CORS gom về một chỗ (mobile gọi qua gateway duy nhất)
const string CorsPolicy = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors(CorsPolicy);
app.MapReverseProxy();

app.Run();
