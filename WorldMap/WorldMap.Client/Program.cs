using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using MagicOnion.Serialization;
using WorldMap.Shared.Interfaces;
using WorldMap.Shared.OnionContracts;
using WorldMap.Shared.Models;

const string ServerBaseAddress = "https://localhost:7145";

// Setup serializer and Grpc channel with MagicOnion using MemoryPack
MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;
var channel = GrpcChannel.ForAddress(ServerBaseAddress);

// Create service client
var mapService = MagicOnionClient.Create<IMapService>(channel);

// Create streaming hub client and receiver implementation
var receiver = new ConsoleMapHubReceiver();
var mapHub = await StreamingHubClient.ConnectAsync<IMapHub, IMapHubReceiver>(channel, receiver);

try
{
    await RunMenuAsync(mapService, mapHub);
}
finally
{
    await mapHub.DisposeAsync();
}

static async Task RunMenuAsync(IMapService mapService, IMapHub mapHub)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("WorldMap Client");
        Console.WriteLine("1 - Join to the room");
        Console.WriteLine("2 - Get regions in area");
        Console.WriteLine("3 - Get objects in area");
        Console.WriteLine("0 - Exit");
        Console.Write("Select option: ");

        var input = Console.ReadLine();
        Console.WriteLine();

        switch (input)
        {
            case "1":
                await JoinRoomAsync(mapHub);
                break;
            case "2":
                await GetRegionsAsync(mapService);
                break;
            case "3":
                await GetObjectsAsync(mapService);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Unknown option.");
                break;
        }
    }
}

static async Task JoinRoomAsync(IMapHub mapHub)
{
    await mapHub.JoinAsync();
    Console.WriteLine("Joined the room. Listening for events. Press Enter to leave...");
    Console.ReadLine();
    await mapHub.LeaveAsync();
    Console.WriteLine("Left the room.");
}

static async Task GetRegionsAsync(IMapService mapService)
{
    var request = new GetRegionsInAreaRequest
    {
        X = ReadInt("Enter X: "),
        Y = ReadInt("Enter Y: "),
        Width = ReadInt("Enter Width: "),
        Height = ReadInt("Enter Height: ")
    };

    var response = await mapService.GetRegionsInAreaAsync(request);

    if (response.Regions == null || response.Regions.Count == 0)
    {
        Console.WriteLine("No regions found.");
        return;
    }

    Console.WriteLine($"Regions ({response.Regions.Count}):");
    foreach (var region in response.Regions)
    {
        Console.WriteLine(FormatRegion(region));
    }
}

static async Task GetObjectsAsync(IMapService mapService)
{
    var request = new GetObjectsInAreaRequest
    {
        X = ReadInt("Enter X: "),
        Y = ReadInt("Enter Y: "),
        Width = ReadInt("Enter Width: "),
        Height = ReadInt("Enter Height: ")
    };

    var response = await mapService.GetObjectsInAreaAsync(request);

    if (response.Objects == null || response.Objects.Count == 0)
    {
        Console.WriteLine("No objects found.");
        return;
    }

    Console.WriteLine($"Objects ({response.Objects.Count}):");
    foreach (var obj in response.Objects)
    {
        Console.WriteLine(FormatObject(obj));
    }
}

static int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var line = Console.ReadLine();
        if (int.TryParse(line, out var value))
        {
            return value;
        }
        Console.WriteLine("Invalid number, try again.");
    }
}

static string FormatRegion(RegionDto region)
{
    return $"[{region.Id}] {region.Name} @ ({region.X},{region.Y}) {region.Width}x{region.Height} | {region.Metadata}";
}

static string FormatObject(MapObjectDto obj)
{
    return $"[{obj.Id}] @ ({obj.X},{obj.Y}) {obj.Width}x{obj.Height}";
}

public class ConsoleMapHubReceiver : IMapHubReceiver
{
    public void OnObjectAdded(ObjectAddedEvent evt)
    {
        Console.WriteLine($"[EVENT] Object Added: {evt.Object.Id} @ ({evt.Object.X},{evt.Object.Y}) {evt.Object.Width}x{evt.Object.Height}");
    }

    public void OnObjectDeleted(ObjectDeletedEvent evt)
    {
        Console.WriteLine($"[EVENT] Object Deleted: {evt.Id}");
    }

    public void OnObjectUpdated(ObjectUpdatedEvent evt)
    {
        Console.WriteLine($"[EVENT] Object Updated: {evt.Object.Id} @ ({evt.Object.X},{evt.Object.Y}) {evt.Object.Width}x{evt.Object.Height}");
    }
}

