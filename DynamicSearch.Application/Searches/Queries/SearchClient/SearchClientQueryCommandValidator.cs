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

            RuleFor(e => e.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(e => e.PageLimit)
               .GreaterThanOrEqualTo(1);
        }
    }
}