namespace GeoJsonRandom.Models
{
    public class GeoDataResultModel
    {
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? County { get; set; }
        public string? Town { get; set; }
        public string? Village { get; set; }
    }

    public class GeoDataConditionModel
    {
        public string? County { get; set; }
        public string? Town { get; set; }
        public string? Village { get; set; }
        public int TakeCount { get; set; }
    }
}
