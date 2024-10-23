using GeoJsonRandom.Core.Models;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace GeoJsonRandom.Core.Services
{
    public class GeoJsonPrefixTreeBuilder : IGeoJsonPrefixTreeBuilder
    {
        private string _geojsonPath;
        private readonly Lazy<GeoTreeNode> _root;

        public GeoJsonPrefixTreeBuilder(string geojsonPath)
        {
            _geojsonPath = geojsonPath;
            _root = new Lazy<GeoTreeNode>(InitData, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary> 回傳PreifxTree (如未建立則先建立) </summary>
        public GeoTreeNode GetOrBuildTree() => _root.Value;

        private GeoTreeNode InitData()
        {
            GeoTreeNode root = new GeoTreeNode { Name = "Root" };
            FeatureCollection? featureCollection = LoadGeoJson(_geojsonPath);
            if (featureCollection == null)
                return root;

            foreach (IFeature feature in featureCollection)
                AddFeatureToTree(root, feature);
            return root;
        }

        private static FeatureCollection? LoadGeoJson(string filePath)
        {
            //using var reader = new StringReader(File.ReadAllText(filePath));
            using var reader = new StreamReader(filePath);
            using var jsonReader = new JsonTextReader(reader);
            JsonSerializer serializer = GeoJsonSerializer.Create();
            return serializer.Deserialize<FeatureCollection>(jsonReader);
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
