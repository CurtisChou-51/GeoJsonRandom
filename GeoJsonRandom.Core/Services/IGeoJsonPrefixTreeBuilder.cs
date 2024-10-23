using GeoJsonRandom.Core.Models;

namespace GeoJsonRandom.Core.Services
{
    public interface IGeoJsonPrefixTreeBuilder
    {
        GeoTreeNode GetOrBuildTree();
    }
}