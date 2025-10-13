using MagicOnion;

namespace WorldMap.Shared.Interfaces
{
    public interface IMapHub : IStreamingHub<IMapHub, IMapHubReceiver>
    {
        Task JoinAsync();
        Task LeaveAsync();
    }
}

