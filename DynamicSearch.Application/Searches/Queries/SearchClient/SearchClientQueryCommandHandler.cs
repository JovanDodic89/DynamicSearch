using DynamicSearch.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Search.By.String.Extensions;
using System.Reflection;

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
            IQueryable<object> query = _quearableProviderRepository.Get(request.ClientName, request.SchemaName, request.EntityName);

            MethodInfo methodInfo = typeof(SearchClientQueryCommandHandler).GetMethod(nameof(Handle), BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = methodInfo.MakeGenericMethod(query.ElementType);

            Task getResultTask = (Task)generic.Invoke(this, new object[]{ query, request, cancellationToken });
            await getResultTask.ConfigureAwait(false);

            return (SearchClientResponseDto)getResultTask.GetType().GetProperty("Result").GetValue(getResultTask);
        }

        private async Task<SearchClientResponseDto> Handle<TSource>(IQueryable<object> query, SearchClientQueryCommand request, CancellationToken cancellationToken)
            where TSource : class
        {
            IQueryable<TSource> sourceQuery = query as IQueryable<TSource>;

            sourceQuery = sourceQuery.Where(request.SearchString);

            return new SearchClientResponseDto
            {
                TotalCount = await sourceQuery.CountAsync(cancellationToken),
                Items = (await sourceQuery.Skip((request.Page - 1) * request.PageLimit)
                                  .Take(request.PageLimit)
                                  .ToListAsync<object>(cancellationToken))
                                  .AsReadOnly(),
                Page = request.Page,
                PageLimit = request.PageLimit
            };
        }
    }
}
