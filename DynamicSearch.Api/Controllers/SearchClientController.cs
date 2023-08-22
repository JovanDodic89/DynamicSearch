using ABC.Microservices.Common.Extensions.API.ModelBinders;
using DynamicSearch.Application.Searches.Queries.SearchClient;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DynamicSearch.Api.Controllers
{
    [Route("api/v1/search")]
    public class SearchClientController : Controller
    {
        private readonly IMediator _mediator;

        public SearchClientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{clientName}/{schemaName}/{entityName}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<SearchClientResponseDto>> Search([ModelBinder(typeof(QueryCommandModelBinder))] SearchClientQueryCommand searchClientQueryCommand)
        {
            var result = await _mediator.Send(searchClientQueryCommand);

            if (result.TotalCount == 0)
            {
                return NoContent();
            }

           return Ok(result);
        }
    }
}