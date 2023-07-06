using DynamicSearch.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
