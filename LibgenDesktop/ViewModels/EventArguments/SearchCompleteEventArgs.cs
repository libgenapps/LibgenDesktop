using System;
using System.Collections.Generic;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels.EventArguments
{
    internal class SearchCompleteEventArgs<T> : EventArgs where T: LibgenObject
    {
        public SearchCompleteEventArgs(string searchQuery, List<T> searchResult)
        {
            SearchQuery = searchQuery;
            SearchResult = searchResult;
        }

        public string SearchQuery { get; }
        public List<T> SearchResult { get; }
    }
}
