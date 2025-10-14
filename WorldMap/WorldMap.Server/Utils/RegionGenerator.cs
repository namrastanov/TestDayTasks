using System.Drawing;
using WorldMap.Domain;

namespace WorldMap.Server.Utils
{
    public static class RegionGenerator
    {
        public static Region[] GenerateGrid(MapOptions mapOptions)
        {
            if (mapOptions.Width <= 0) throw new ArgumentOutOfRangeException(nameof(mapOptions.Width));
            if (mapOptions.Height <= 0) throw new ArgumentOutOfRangeException(nameof(mapOptions.Height));
            if (mapOptions.MinimumRegions <= 0) throw new ArgumentOutOfRangeException(nameof(mapOptions.MinimumRegions));

            var columns = (int)Math.Ceiling(Math.Sqrt(mapOptions.MinimumRegions));
            var rows = (int)Math.Ceiling((double)mapOptions.MinimumRegions / columns);
            var cellWidth = mapOptions.Width / columns;
            var cellHeight = mapOptions.Height / rows;

            var actualCount = Math.Min(rows * columns, mapOptions.MinimumRegions);
            var regions = new Region[actualCount];

            ushort id = 1;
            int index = 0;

            for (var row = 0; row < rows && index < actualCount; row++)
            {
                var y = row * cellHeight;
                var height = (row == rows - 1) ? mapOptions.Height - y : cellHeight;

                for (var col = 0; col < columns && index < actualCount; col++)
                {
                    var x = col * cellWidth;
                    var width = (col == columns - 1) ? mapOptions.Width - x : cellWidth;

                    regions[index++] = new Region(id++, $"Region {id - 1}", new Rectangle(x, y, width, height))
                    {
                        Metadata = "Generated"
                    };
                }
            }

            return regions;
        }
    }
}


