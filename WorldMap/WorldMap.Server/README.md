# WorldMap Server - MagicOnion Network Layer

This project implements a high-performance network layer for a tile-based game map using MagicOnion and MemoryPack serialization.

## Architecture Overview

The solution consists of three main components:

### 1. WorldMap.Shared
Contains shared contracts, DTOs, and interfaces used by both client and server:

- **Models**: DTOs serialized with MemoryPack
  - `MapObjectDto`: Represents a map object with position and dimensions
  - `RegionDto`: Represents a region with metadata

- **OnionContracts**: MagicOnion request/response contracts
  - `GetObjectsInAreaRequest/Response`: Query objects in a rectangular area
  - `GetRegionsInAreaRequest/Response`: Query regions in a rectangular area
  - `ObjectAddedEvent`: Notification when an object is added
  - `ObjectUpdatedEvent`: Notification when an object is updated
  - `ObjectDeletedEvent`: Notification when an object is deleted

- **Interfaces**: MagicOnion service and hub interfaces
  - `IMapService`: Unary RPC service for queries
  - `IMapHub`: Streaming hub for real-time notifications
  - `IMapHubReceiver`: Client-side receiver interface

### 2. WorldMap.Layers
Contains the data layer with in-memory storage:

- **ObjectsLayer**: Manages game objects
  - `IObjectLayer`: Interface for object operations
  - `ObjectLayer`: Thread-safe implementation using ConcurrentDictionary
  - Supports spatial queries and change subscriptions

- **RegionsLayer**: Manages regions/countries
  - `IRegionLayer`: Interface for region operations
  - `RegionLayer`: Thread-safe implementation

### 3. WorldMap.Server
Contains the MagicOnion service implementations:

- **Services/MapService**: Implements `IMapService` for unary RPCs
  - `GetObjectsInAreaAsync`: Returns all objects in a specified area
  - `GetRegionsInAreaAsync`: Returns all regions in a specified area

- **Hubs/MapHub**: Implements `IMapHub` for real-time notifications
  - Clients join a "MapRoom" group
  - Automatically broadcasts object changes to all connected clients
  - Implements `IObjectChangeHandler` to receive layer notifications

## Key Features

### Performance Optimizations

1. **MemoryPack Serialization**: Ultra-fast binary serialization with zero allocation
2. **Spatial Queries**: Efficient area-based queries with overlap detection
3. **Thread-Safe Operations**: Using ConcurrentDictionary for lock-free reads
4. **Async/Await**: Non-blocking I/O throughout the stack

### Real-Time Notifications

The hub system provides automatic notifications:
- When a client joins, it subscribes to object changes
- Any object addition, update, or deletion is broadcast to all connected clients
- Clients automatically unsubscribe on disconnect

### Thread Safety

- All layer operations are thread-safe
- Notification handlers are invoked in parallel
- Concurrent operations are properly synchronized

## Usage Example

### Server Startup

The server is configured in `Program.cs`:

```csharp
// Configure MagicOnion with MemoryPack
builder.Services.AddMagicOnion(options =>
{
    options.MessageSerializer = MemoryPackMagicOnionSerializerProvider.Instance;
});

// Register layers as singletons
builder.Services.AddSingleton<IObjectLayer, ObjectLayer>();
builder.Services.AddSingleton<IRegionLayer, RegionLayer>();
```

### Client Usage (Conceptual)

```csharp
// Connect to the service
var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = MagicOnionClient.Create<IMapService>(channel);

// Query objects in an area
var request = new GetObjectsInAreaRequest { X1 = 0, Y1 = 0, X2 = 100, Y2 = 100 };
var response = await client.GetObjectsInAreaAsync(request);

// Connect to the hub for real-time updates
var hubConnection = await StreamingHubClient.ConnectAsync<IMapHub, IMapHubReceiver>(
    channel, 
    receiver);

await hubConnection.JoinAsync();

// Receiver implementation
class MapHubReceiver : IMapHubReceiver
{
    public void OnObjectAdded(ObjectAddedEvent evt) 
    {
        Console.WriteLine($"Object added: {evt.Object.Id}");
    }
    
    public void OnObjectUpdated(ObjectUpdatedEvent evt) 
    {
        Console.WriteLine($"Object updated: {evt.Object.Id}");
    }
    
    public void OnObjectDeleted(ObjectDeletedEvent evt) 
    {
        Console.WriteLine($"Object deleted: {evt.Id}");
    }
}
```

## Testing

Comprehensive unit tests are provided in `WorldMap.Server.Tests`:

- **MapServiceTests**: Tests for the unary RPC service
- **ObjectLayerTests**: Tests for object layer operations, including thread safety
- **RegionLayerTests**: Tests for region layer operations

Run tests with:
```bash
dotnet test WorldMap.Server.Tests
```

## API Endpoints

### Unary RPCs (IMapService)

#### GetObjectsInArea
- **Request**: `GetObjectsInAreaRequest { X1, Y1, X2, Y2 }`
- **Response**: `GetObjectsInAreaResponse { List<MapObjectDto> Objects }`
- **Description**: Returns all objects overlapping with the specified rectangular area

#### GetRegionsInArea
- **Request**: `GetRegionsInAreaRequest { X1, Y1, X2, Y2 }`
- **Response**: `GetRegionsInAreaResponse { List<RegionDto> Regions }`
- **Description**: Returns all regions overlapping with the specified rectangular area

### Streaming Hub (IMapHub)

#### Join
- **Description**: Subscribes the client to real-time object change notifications

#### Leave
- **Description**: Unsubscribes the client from notifications

#### Events (Server → Client)
- `OnObjectAdded(ObjectAddedEvent)`: Fired when an object is added
- `OnObjectUpdated(ObjectUpdatedEvent)`: Fired when an object is updated
- `OnObjectDeleted(ObjectDeletedEvent)`: Fired when an object is deleted

## Configuration

### CORS
CORS is enabled for all origins in development. For production, configure appropriate CORS policies in `Program.cs`.

### Logging
The server uses built-in ASP.NET Core logging. Configure log levels in `appsettings.json`.

## Extensibility

The architecture is designed for easy extension:

1. **Add New Queries**: Implement new methods in `IMapService` and `MapService`
2. **Add New Events**: Add new event types in `OnionContracts` and methods in `IMapHubReceiver`
3. **Add New Layers**: Create new layer interfaces and implementations following the existing pattern
4. **Custom Serialization**: MemoryPack handles most types automatically, but custom formatters can be added

## Performance Considerations

1. **Spatial Queries**: Current implementation uses linear search. For production with many objects, consider:
   - Spatial indexing (R-tree, Quadtree)
   - Chunking the world into sectors

2. **Broadcasting**: All clients receive all notifications. For large-scale deployments, consider:
   - Area-of-interest filtering
   - Multiple hub groups based on world sectors

3. **Memory**: In-memory storage is suitable for development. For production, consider:
   - Persistent storage (database)
   - Caching layer

## Dependencies

- **MagicOnion.Server** (6.1.3): gRPC-based RPC framework
- **MemoryPack** (1.21.1): High-performance serialization
- **ASP.NET Core** (8.0): Web framework

## License

[Your License Here]

