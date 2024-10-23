namespace GeoJsonRandom.Models
{
    public class GeoDataResultDto
    {
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? County { get; set; }
        public string? Town { get; set; }
        public string? Village { get; set; }
    }

    public class GeoDataConditionDto
    {
        public string? County { get; set; }
        public string? Town { get; set; }
        public string? Village { get; set; }
        public int TakeCount { get; set; }
    }
}
