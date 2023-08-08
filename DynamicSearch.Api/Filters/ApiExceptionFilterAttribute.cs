using DynamicSearch.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DynamicSearch.Api.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilterAttribute()
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                //{ typeof(NotFoundException), HandleNotFoundException },
                { typeof(RequestNotValidException), HandleRequestNotValidException }
            };
        }

        private void HandleUnauthorizedEnviorimentTokenException(ExceptionContext obj)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            };

            obj.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

            obj.ExceptionHandled = true;
        }

        private void HandleRequestNotValidException(ExceptionContext obj)
        {
            ValidationProblemDetails problemDetails = new ValidationProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            var errors = problemDetails.Errors;
            errors.Add(((RequestNotValidException)obj.Exception).Field, new string[] { obj.Exception.Message });

            obj.Result = new BadRequestObjectResult(problemDetails);

            obj.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext obj)
        {
            ProblemDetails problemDetails = new ProblemDetails
            {
                Title = "The specified resource was not found.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Detail = obj.Exception.Message
            };

            obj.Result = new NotFoundObjectResult(problemDetails);

            obj.ExceptionHandled = true;
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();

            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);

                return;
            }
            else
            {
                ProblemDetails problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred while processing your request. Please contact system administrator.",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Detail = context.Exception.Message
                };

                context.Result = new ObjectResult(problemDetails)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            context.ExceptionHandled = true;
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);

            base.OnException(context);
        }
    }
}
