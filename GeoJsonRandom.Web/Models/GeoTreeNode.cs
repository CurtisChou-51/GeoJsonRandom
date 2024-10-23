using NetTopologySuite.Geometries;

namespace GeoJsonRandom.Models
{
    public class GeoTreeNode
    {
        public string? County { get; set; }
        public string? Town { get; set; }
        public string? Village { get; set; }
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, GeoTreeNode> Children { get; } = new Dictionary<string, GeoTreeNode>();
        public Geometry? Geo { get; set; }
        public bool IsLeaf => Children.Count == 0;
        public double Area { get; set; }
    }
}
