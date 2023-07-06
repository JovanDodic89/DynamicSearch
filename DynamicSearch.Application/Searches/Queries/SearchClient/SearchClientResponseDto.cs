using System.Collections.ObjectModel;

namespace DynamicSearch.Application.Searches.Queries.SearchClient
{
    public class SearchClientResponseDto
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageLimit { get; set; }
        public ReadOnlyCollection<object> Items { get; set; }     
    }
}
