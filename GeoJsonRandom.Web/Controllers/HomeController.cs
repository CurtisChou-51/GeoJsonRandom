using GeoJsonRandom.Models;
using GeoJsonRandom.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoJsonRandom.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGeoDataService _geoDataService;

        public HomeController(ILogger<HomeController> logger, IGeoDataService geoDataService)
        {
            _logger = logger;
            _geoDataService = geoDataService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Counties()
        {
            var result = _geoDataService.GetCounties().ToList();
            return Ok(new { data = result });
        }

        [HttpPost]
        public IActionResult Towns(GeoDataConditionDto dto)
        {
            var result = _geoDataService.GetTowns(dto.County).ToList();
            return Ok(new { data = result });
        }

        [HttpPost]
        public IActionResult Villages(GeoDataConditionDto dto)
        {
            var result = _geoDataService.GetVillages(dto.County, dto.Town).ToList();
            return Ok(new { data = result });
        }

        [HttpPost]
        public IActionResult RandomPoints(GeoDataConditionDto dto)
        {
            var result = _geoDataService.GenerateRandomPoints(dto).ToList();
            return Ok(new { data = result });
        }

        [HttpPost]
        public IActionResult RandomPointsJsonFile(GeoDataConditionDto dto)
        {
            var stream = _geoDataService.GenerateRandomPointsJsonFile(dto);
            return File(stream, "application/json", "data.json");
        }

        [HttpPost]
        public IActionResult RandomPointsCsvFile(GeoDataConditionDto dto)
        {
            var stream = _geoDataService.GenerateRandomPointsCsvFile(dto);
            return File(stream, "text/csv", "data.csv");
        }
    }
}
