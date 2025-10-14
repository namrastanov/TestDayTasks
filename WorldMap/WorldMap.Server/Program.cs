using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Drawing;
using WorldMap.Domain;
using WorldMap.Application;
using WorldMap.Infrastructure;
using WorldMap.Server.Utils;

var builder = WebApplication.CreateBuilder(args);

// Configure MagicOnion with MemoryPack serialization
builder.Services.AddMagicOnion(options =>
{
    options.MessageSerializer = MemoryPackMagicOnionSerializerProvider.Instance;
});

// Register layer services as singletons for in-memory state
var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var options = ConfigurationOptions.Parse(redisConnectionString);
    options.AbortOnConnectFail = false; // allow app to start if Redis is temporarily unavailable
    return ConnectionMultiplexer.Connect(options);
});
builder.Services.AddSingleton<IObjectRepository<GameObject>, RedisObjectRepository<GameObject>>();
builder.Services.AddSingleton<IObjectLayer, ObjectLayer>();

// Bind typed Map options and register regions layer using typed values
builder.Services.Configure<MapOptions>(builder.Configuration.GetSection("Map"));
builder.Services.AddSingleton<IRegionsLayer>(sp =>
{
    var mapOptions = sp.GetRequiredService<IOptions<MapOptions>>().Value;
    var initialRegions = RegionGenerator.GenerateGrid(mapOptions);
    return new RegionLayer(initialRegions.AsSpan(), mapOptions.Width, mapOptions.Height);
});

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});

// Add gRPC services
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapMagicOnionService();

app.Run();
