using GeoJsonRandom.Core.Models;
using GeoJsonRandom.Models;

namespace GeoJsonRandom.Services
{
    public interface IGeoDataService
    {
        /// <summary> 取得縣市 </summary>
        IEnumerable<string> GetCounties();

        /// <summary> 取得鄉鎮 </summary>
        IEnumerable<string> GetTowns(string? county);

        /// <summary> 取得村里 </summary>
        IEnumerable<string> GetVillages(string? county, string? town);

        /// <summary> 產生隨機點位 </summary>
        IEnumerable<GeoDataResultDto> GenerateRandomPoints(GeoDataConditionModel vm);

        /// <summary> 產生隨機點位JsonFile </summary>
        Stream GenerateRandomPointsJsonFile(GeoDataConditionModel vm);

        /// <summary> 產生隨機點位CsvFile </summary>
        Stream GenerateRandomPointsCsvFile(GeoDataConditionModel vm);
    }
}