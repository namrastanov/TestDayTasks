using WorldMap.Domain;

namespace WorldMap.Application
{
    public class RegionLayer : IRegionsLayer
    {
        private Region[] _regions;
        private ushort[] _regionIds;

        public int Width { get; }
        public int Height { get; }

        public RegionLayer(Span<Region> initialRegions, int width, int height)
        {
            if (initialRegions.IsEmpty)
                throw new ArgumentException("Переданная коллекция регионов пуста");

            _regions = initialRegions.ToArray();
            _regionIds = new ushort[width * height];

            Width = width;
            Height = height;

            FillRegionIds();
        }

        public string? GetRegionIdAtPosition(int x, int y)
        {
            if (!ValidateCoordinates(x, y))
                return null;

            var index = ToIndex(x, y);

            if (_regionIds.Length <= index)
                return null;

            var regionId = _regionIds[index];

            return regionId.ToString();
        }

        public bool IsTileInRegion(int x, int y)
        {
            if (!ValidateCoordinates(x, y)) return false;

            var index = ToIndex(x, y);

            if (_regionIds.Length <= index)
                return false;

            var regionId = _regionIds[index];

            foreach (var region in _regions)
            {
                if (region.Id == regionId && region.Area.Contains(x, y))
                    return true;
            }

            return false;
        }

        public string? GetRegionMetadataById(string id)
        {
            ushort regionId;
            if (!ushort.TryParse(id, out regionId))
                return null;

            var region = Array.Find(_regions, r => r.Id == regionId);
            return region?.Metadata ?? null;
        }

        public IReadOnlyCollection<Region> GetRegionsIntersectingArea(int topLeftX, int topLeftY, int width, int height)
        {
            var result = new HashSet<Region>();

            for (var x = topLeftX; x < topLeftX + width; x++)
            {
                for (var y = topLeftY; y < topLeftY + height; y++)
                {
                    if (!ValidateCoordinates(x, y)) continue;

                    var index = ToIndex(x, y);

                    if (_regionIds.Length <= index)
                        continue;

                    var regionId = _regionIds[index];

                    var region = Array.Find(_regions, r => r.Id == regionId);
                    if (region != null)
                        result.Add(region);
                }
            }

            return result;
        }

        private void FillRegionIds()
        {
            foreach (var region in _regions)
            {
                for (var x = region.Area.Left; x < region.Area.Right; x++)
                {
                    for (var y = region.Area.Top; y < region.Area.Bottom; y++)
                    {
                        if (!ValidateCoordinates(x, y)) continue;

                        var index = ToIndex(x, y);
                        _regionIds[index] = region.Id;
                    }
                }
            }
        }

        private int ToIndex(int x, int y) => y * Width + x;

        private bool ValidateCoordinates(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height;
    }
}
