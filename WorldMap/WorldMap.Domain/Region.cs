using System.Drawing;
using WorldMap.Core.Shared;

namespace WorldMap.Domain
{
    public class Region : BaseObject<ushort>
    {
        public string Name { get; set; } = String.Empty;
        public string Metadata { get; set; } = String.Empty;
        public Rectangle Area { get; private set; }

        public Region(ushort id, string name, Rectangle area)
        {
            Id = id;
            Name = name;
            Area = area;
            X = area.X;
            Y = area.Y;
            Width = area.Width;
            Height = area.Height;
        }
    }
}