using System.Drawing;
using WorldMap.Core.Shared;

namespace WorldMap.Domain
{
    public class Region : BaseObject<ushort>
    {
        public string Name { get; set; } = String.Empty;
        public string Metadata { get; set; } = String.Empty;
        
        public Region(ushort id, string name, int topLeftX, int topLeftY, int width, int height)
        {
            Id = id;
            Name = name;
            X = topLeftX;
            Y = topLeftY;
            Width = width;
            Height = height;
        }        
    }
}