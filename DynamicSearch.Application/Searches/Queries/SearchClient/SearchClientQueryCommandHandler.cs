using DynamicSearch.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Search.By.String.Extensions;

namespace DynamicSearch.Application.Searches.Queries.SearchClient
{
    public class SearchClientQueryCommandHandler : IRequestHandler<SearchClientQueryCommand, SearchClientResponseDto>
    {
        private readonly IQuearableProviderRepository _quearableProviderRepository;

        public SearchClientQueryCommandHandler(IQuearableProviderRepository quearableProviderRepository)
        {
            _quearableProviderRepository = quearableProviderRepository;
        }

        public async Task<SearchClientResponseDto> Handle(SearchClientQueryCommand request, CancellationToken cancellationToken)
        {

            _quearableProviderRepository.Get(null, null, null).Where("");

            return new SearchClientResponseDto
            {
                TotalCount = _quearableProviderRepository.Get(null, null, null).Count(),
                Items = (await _quearableProviderRepository.Get(null, null, null)
                                                           .Skip(request.Page)
                                                           .Take(request.PageLimit)
                                                           .ToListAsync())
                                                           .AsReadOnly(),
                Page = request.Page,
                PageLimit = request.PageLimit
            };
        }
    }
}
