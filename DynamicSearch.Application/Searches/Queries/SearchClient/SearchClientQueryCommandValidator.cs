using FluentValidation;

namespace DynamicSearch.Application.Searches.Queries.SearchClient
{
    public class SearchClientQueryCommandValidator : AbstractValidator<SearchClientQueryCommand>
    {
        public SearchClientQueryCommandValidator()
        {
            RuleFor(e => e.ClientName)
                .NotEmpty()
                .MaximumLength(12);
        }
    }
}