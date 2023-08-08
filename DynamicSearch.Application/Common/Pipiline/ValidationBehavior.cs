﻿using DynamicSearch.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace DynamicSearch.Application.Common.Pipeline
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        { 
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var failiures = _validators.Select(v => v.Validate(context))
                                        .SelectMany(x => x.Errors)
                                        .Where(f => f != null)
                                        .ToList();
            
            if (failiures.Count > 0)
            {
                throw new RequestNotValidException(failiures[0].PropertyName, failiures[0].ErrorMessage);
            }

            return await next.Invoke();
        }
    }
}
