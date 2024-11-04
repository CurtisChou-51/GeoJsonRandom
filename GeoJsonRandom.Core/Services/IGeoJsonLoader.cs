using NetTopologySuite.Features;

namespace GeoJsonRandom.Core.Services
{
    public interface IGeoJsonLoader
    {
        /// <summary> 讀取 GeoJSON 資料 </summary>
        FeatureCollection? Load();
    }
}