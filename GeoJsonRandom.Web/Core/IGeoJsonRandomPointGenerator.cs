using GeoJsonRandom.Models;

namespace GeoJsonRandom.Core
{
    public interface IGeoJsonRandomPointGenerator
    {
        /// <summary> 取得行政區名稱 </summary>
        IEnumerable<string> GetDirectAdminNames(string?[] adminHierarchy);

        /// <summary> 產生隨機點位 </summary>
        IEnumerable<GeoDataResultDto> GenerateRandomPoints(GeoDataConditionDto dto);
    }
}