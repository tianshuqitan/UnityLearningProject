using System.Collections.Generic;

namespace NewGraph
{
    public interface ISearchWindowProvider
    {
        List<SearchTreeEntry> CreateSearchTree();
    }
}