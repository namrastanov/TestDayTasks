using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using StackExchange.Redis;
using WorldMap.Domain;
using WorldMap.Application;
using WorldMap.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure MagicOnion with MemoryPack serialization
builder.Services.AddMagicOnion(options =>
{
    options.MessageSerializer = MemoryPackMagicOnionSerializerProvider.Instance;
});

// Register layer services as singletons for in-memory state
var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
builder.Services.AddSingleton(sp => ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddSingleton<IObjectRepository<GameObject>, RedisObjectRepository<GameObject>>();
builder.Services.AddSingleton<IObjectLayer, ObjectLayer>();
builder.Services.AddSingleton<IRegionsLayer, RegionLayer>();

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
