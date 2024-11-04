using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace GeoJsonRandom.Core.Services
{
    public class FileGeoJsonLoader : IGeoJsonLoader
    {
        private readonly string _filePath;

        public FileGeoJsonLoader(string filePath)
        {
            _filePath = filePath;
        }

        public FeatureCollection? Load()
        {
            using var reader = new StreamReader(_filePath);
            using var jsonReader = new JsonTextReader(reader);
            JsonSerializer serializer = GeoJsonSerializer.Create();
            return serializer.Deserialize<FeatureCollection>(jsonReader);
        }
    }
}