using GeoJsonRandom.Core.Models;

namespace GeoJsonRandom.Core.Services
{
    public interface IGeoJsonPrefixTreeBuilder
    {
        /// <summary> 回傳PreifxTree (如未建立則先建立) </summary>
        GeoTreeNode GetOrBuildTree();
    }
}