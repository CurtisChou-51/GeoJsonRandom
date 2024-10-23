using GeoJsonRandom.Models;

namespace GeoJsonRandom.Core
{
    public interface IGeoJsonPrefixTreeBuilder
    {
        GeoTreeNode GetOrBuildTree();
    }
}