using MediatR;

namespace DynamicSearch.Application.Searches.Queries.SearchClient
{
    public class SearchClientQueryCommand : IRequest<SearchClientResponseDto>
    {
        public string ClientName { get; set; }
        public int Page { get; set; }
        public int PageLimit { get; set; }
        public string SearchString { get; set; }
        public string OrderBy { get; set; }
    }
}
