using GeoJsonRandom.Core.Models;

namespace GeoJsonRandom.Core.Services
{
    public interface IGeoJsonRandomPointGenerator
    {
        /// <summary> 取得行政區名稱 </summary>
        IEnumerable<string> GetDirectAdminNames(string?[] adminHierarchy);

        /// <summary> 產生隨機點位 </summary>
        IEnumerable<GeoDataResultDto> GenerateRandomPoints(string?[] adminHierarchy, int takeCount);
    }
}