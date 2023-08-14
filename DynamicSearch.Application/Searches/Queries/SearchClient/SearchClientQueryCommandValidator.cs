using FluentValidation;

namespace DynamicSearch.Application.Searches.Queries.SearchClient
{
    public class SearchClientQueryCommandValidator : AbstractValidator<SearchClientQueryCommand>
    {
        public SearchClientQueryCommandValidator()
        {
            RuleFor(e => e.ClientName)
                .NotEmpty();

            RuleFor(e => e.SchemaName)
                .NotEmpty();

            RuleFor(e => e.EntityName)
                .NotEmpty();
        }
    }
}