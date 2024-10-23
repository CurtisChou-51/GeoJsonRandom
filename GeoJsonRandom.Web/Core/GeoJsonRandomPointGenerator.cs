using GeoJsonRandom.Models;

namespace GeoJsonRandom.Core
{
    public class GeoJsonRandomPointGenerator : IGeoJsonRandomPointGenerator
    {
        private readonly IGeoJsonPrefixTreeBuilder _geoJsonPrefixTreeBuilder;

        private GeoTreeNode Root => _geoJsonPrefixTreeBuilder.GetOrBuildTree();

        public GeoJsonRandomPointGenerator(IGeoJsonPrefixTreeBuilder geoJsonPrefixTreeBuilder)
        {
            _geoJsonPrefixTreeBuilder = geoJsonPrefixTreeBuilder;
        }

        /// <summary> 取得行政區名稱 </summary>
        public IEnumerable<string> GetDirectAdminNames(string?[] adminHierarchy)
        {
            GeoTreeNode? node = FindNode(adminHierarchy);
            if (node == null)
                yield break;
            foreach (var name in node.Children.Keys)
                yield return name;
        }

        /// <summary> 產生隨機點位 </summary>
        public IEnumerable<GeoDataResultDto> GenerateRandomPoints(GeoDataConditionDto dto)
        {
            GeoTreeNode? node = FindNode([dto.County, dto.Town, dto.Village]);
            if (node == null)
                yield break;
            int takeCount = dto.TakeCount;
            while (takeCount > 0)
            {
                GeoTreeNode leaf = GetRandomLeafFromNode(node);
                if (leaf.Geo == null)
                    throw new Exception("leaf geo not init");
                (decimal lng, decimal lat) = RandomHelper.GenerateLngLatIn(leaf.Geo);
                yield return new GeoDataResultDto
                {
                    County = leaf.County,
                    Town = leaf.Town,
                    Village = leaf.Village,
                    Latitude = lat,
                    Longitude = lng
                };
                takeCount--;
            }
        }

        /// <summary> 依據行政區層級尋找節點 </summary>
        private GeoTreeNode? FindNode(string?[] adminHierarchy)
        {
            GeoTreeNode currentNode = Root;
            foreach (string? name in adminHierarchy)
            {
                if (string.IsNullOrEmpty(name))
                    break;
                if (!currentNode.Children.TryGetValue(name, out GeoTreeNode? value))
                    return null;
                currentNode = value;
            }
            return currentNode;
        }

        /// <summary> 由目前node取得隨機leaf </summary>
        private static GeoTreeNode GetRandomLeafFromNode(GeoTreeNode node)
        {
            GeoTreeNode currentNode = node;
            while (!currentNode.IsLeaf)
                currentNode = RandomHelper.SelectItemByWeight(currentNode.Children.Values, x => x.Area);
            return currentNode;
        }
    }
}
