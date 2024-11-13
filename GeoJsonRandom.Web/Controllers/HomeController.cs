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


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Counties()
        {
            var result = _geoDataService.GetCounties().ToList();
            return Ok(new { data = result });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Towns(GeoDataConditionModel vm)
        {
            var result = _geoDataService.GetTowns(vm.County).ToList();
            return Ok(new { data = result });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Villages(GeoDataConditionModel vm)
        {
            var result = _geoDataService.GetVillages(vm.County, vm.Town).ToList();
            return Ok(new { data = result });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult RandomPoints(GeoDataConditionModel vm)
        {
            var result = _geoDataService.GenerateRandomPoints(vm).ToList();
            return Ok(new { data = result });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult RandomPointsJsonFile(GeoDataConditionModel vm)
        {
            var stream = _geoDataService.GenerateRandomPointsJsonFile(vm);
            return File(stream, "application/json", "data.json");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult RandomPointsCsvFile(GeoDataConditionModel vm)
        {
            var stream = _geoDataService.GenerateRandomPointsCsvFile(vm);
            return File(stream, "text/csv", "data.csv");
        }
    }
}
