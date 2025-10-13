using StackExchange.Redis;
using WorldMap.Domain;

namespace WorldMap.Infrastructure
{
    public class RedisObjectRepository<T> : IRedisObjectRepository<T> where T : BaseObject, new()
    {
        private const string ObjectLocationKey = "object_locations";
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisObjectRepository(ConnectionMultiplexer redis)
        {
            _redis = redis ??
                throw new ArgumentNullException(nameof(redis));

            _db = _redis.GetDatabase();
        }

        public async Task AddAsync(T obj)
        {
            var geoCoordinates = CoordinateConverter.ToGeoCoordinates(obj.X, obj.Y);
            await _db.GeoAddAsync(ObjectLocationKey, new[] { new GeoEntry(geoCoordinates.Longitude, geoCoordinates.Latitude, obj.Id) });

            await _db.HashSetAsync($"obj:{obj.Id}",
            [
            new("id", obj.Id),
            new("x", obj.X),
            new("y", obj.Y),
            new("width", obj.Width),
            new("height", obj.Height)
            ]);
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            if (!await _db.KeyExistsAsync($"obj:{id}"))
                return null;

            var hashData = await _db.HashGetAllAsync($"obj:{id}");
            return new T
            {
                Id = hashData[0].Value,
                X = Convert.ToInt32(hashData[1].Value),
                Y = Convert.ToInt32(hashData[2].Value),
                Width = Convert.ToInt32(hashData[3].Value),
                Height = Convert.ToInt32(hashData[4].Value)
            };
        }

        public async Task RemoveAsync(string id)
        {
            await _db.KeyDeleteAsync($"obj:{id}");
            await _db.GeoRemoveAsync(ObjectLocationKey, id);
        }

        public async Task<IEnumerable<T>> GetByAreaAsync(int topLeftX, int topLeftY, int width, int height)
        {
            var bottomRightX = topLeftX + width;
            var bottomRightY = topLeftY + height;

            var centerLatLng = CoordinateConverter.ToGeoCoordinates((topLeftX + bottomRightX) / 2, (topLeftY + bottomRightY) / 2);
            var radiusMeters = Math.Sqrt(Math.Pow(bottomRightX - topLeftX, 2) + Math.Pow(bottomRightY - topLeftY, 2)) * 1000;

            var results = await _db.GeoRadiusAsync(
                ObjectLocationKey,
                centerLatLng.Longitude,
                centerLatLng.Latitude,
                radiusMeters,
                unit: GeoUnit.Meters,
                order: Order.Ascending);

            var objects = new List<T>();
            foreach (var result in results)
            {
                var obj = await GetByIdAsync(result.Member);
                if (obj != null && CheckIfInsideAreaAsync(obj, topLeftX, topLeftY, bottomRightX, bottomRightY))
                    objects.Add(obj);
            }

            return objects;
        }

        public async Task<T?> GetByCoordinatesAsync(int x, int y)
        {
            var geoCoordinates = CoordinateConverter.ToGeoCoordinates(x, y);
            var nearbyResults = await _db.GeoRadiusAsync(ObjectLocationKey, geoCoordinates.Longitude, geoCoordinates.Latitude, 1, unit: GeoUnit.Meters);

            if (nearbyResults.Length > 0)
            {
                var firstResult = nearbyResults.FirstOrDefault(); // Берём ближайший объект
                return await GetByIdAsync(firstResult.Member);
            }
            else
            {
                return null;
            }
        }

        public bool CheckIfInsideAreaAsync(T obj, int topLeftX, int topLeftY, int width, int height)
        {
            var bottomRightX = topLeftX + width;
            var bottomRightY = topLeftY + height;
            return !(
                obj.X >= bottomRightX ||
                obj.X + obj.Width <= topLeftX ||
                obj.Y >= bottomRightY ||
                obj.Y + obj.Height <= topLeftY
            );
        }
    }

    public static class CoordinateConverter
    {
        private const double MapWidth = 1000;
        private const double MapHeight = 1000;

        public static (double Latitude, double Longitude) ToGeoCoordinates(int x, int y)
        {
            return (-y / MapHeight, x / MapWidth);
        }

        public static (int X, int Y) FromGeoCoordinates(double latitude, double longitude)
        {
            var x = (int)(longitude * MapWidth);
            var y = -(int)(latitude * MapHeight);
            return (x, y);
        }
    }
}