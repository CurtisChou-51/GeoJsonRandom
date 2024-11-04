using GeoJsonRandom.Core.Models;
using NetTopologySuite.Features;

namespace GeoJsonRandom.Core.Services
{
    public class GeoJsonPrefixTreeBuilder : IGeoJsonPrefixTreeBuilder
    {
        private readonly IGeoJsonLoader _loader;
        private readonly Lazy<GeoTreeNode> _root;

        public GeoJsonPrefixTreeBuilder(IGeoJsonLoader loader)
        {
            _loader = loader;
            _root = new Lazy<GeoTreeNode>(InitData, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary> 回傳PreifxTree (如未建立則先建立) </summary>
        public GeoTreeNode GetOrBuildTree() => _root.Value;

        private GeoTreeNode InitData()
        {
            GeoTreeNode root = new GeoTreeNode { Name = "Root" };
            FeatureCollection? featureCollection = _loader.Load();
            if (featureCollection == null)
                return root;

            foreach (IFeature feature in featureCollection)
                AddFeatureToTree(root, feature);
            return root;
        }

        private static void AddFeatureToTree(GeoTreeNode root, IFeature feature)
        {
            string? county = feature.Attributes["COUNTYNAME"]?.ToString();
            string? town = feature.Attributes["TOWNNAME"]?.ToString();
            string? village = feature.Attributes["VILLNAME"]?.ToString();
            if (string.IsNullOrEmpty(county) || string.IsNullOrEmpty(town) || string.IsNullOrEmpty(village))
                return;

            GeoTreeNode countyNode = GetOrCreateChild(root, county);
            GeoTreeNode townNode = GetOrCreateChild(countyNode, town);
            GeoTreeNode villageNode = GetOrCreateChild(townNode, village);
            villageNode.County = county;
            villageNode.Town = town;
            villageNode.Village = village;
            villageNode.Geo = feature.Geometry;

            double area = feature.Geometry.Area;
            villageNode.Area = area;
            townNode.Area += area;
            countyNode.Area += area;
            root.Area += area;
        }

        private static GeoTreeNode GetOrCreateChild(GeoTreeNode parent, string name)
        {
            if (!parent.Children.ContainsKey(name))
                parent.Children[name] = new GeoTreeNode { Name = name };
            return parent.Children[name];
        }
    }
}
