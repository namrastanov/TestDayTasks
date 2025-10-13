using MagicOnion;
using WorldMap.Shared.OnionContracts;

namespace WorldMap.Shared.Interfaces
{
    public interface IMapService : IService<IMapService>
    {
        UnaryResult<GetObjectsInAreaResponse> GetObjectsInAreaAsync(GetObjectsInAreaRequest request);
        UnaryResult<GetRegionsInAreaResponse> GetRegionsInAreaAsync(GetRegionsInAreaRequest request);
    }
}

