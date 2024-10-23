using GeoJsonRandom.Models;
using GeoJsonRandom.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoJsonRandom.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GeoDataController : ControllerBase
    {
        private readonly IGeoDataService _geoDataService;

        public GeoDataController(IGeoDataService geoDataService)
        {
            _geoDataService = geoDataService;
        }

        [HttpGet("{takeCount}/{county?}/{town?}/{village?}")]
        public IActionResult RandomPoints(int takeCount, string? county, string? town, string? village)
        {
            var dto = new GeoDataConditionDto { TakeCount = takeCount, County = county, Town = town, Village = village };
            List<GeoDataResultDto> result = _geoDataService.GenerateRandomPoints(dto).ToList();
            return Ok(result);
        }
    }
}
