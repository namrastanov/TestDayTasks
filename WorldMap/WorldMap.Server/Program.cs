using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using MagicOnion.Server;
using WorldMap.Layers;
using WorldMap.Layers.ObjectsLayer;
using WorldMap.Layers.RegionsLayer;

var builder = WebApplication.CreateBuilder(args);

// Configure MagicOnion with MemoryPack serialization
builder.Services.AddMagicOnion(options =>
{
    options.MessageSerializer = MemoryPackMagicOnionSerializerProvider.Instance;
});

// Register layer services as singletons for in-memory state
builder.Services.AddSingleton<IObjectLayer, ObjectLayer>();
builder.Services.AddSingleton<IRegionLayer, RegionLayer>();

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
