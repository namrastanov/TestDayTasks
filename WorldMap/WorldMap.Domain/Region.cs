using WorldMap.Core.Shared;

namespace WorldMap.Domain
{
    public class Region : BaseObject
    {
        public string Name { get; set; } = string.Empty;
        public string Metadata { get; set; } = string.Empty;
    }
}
