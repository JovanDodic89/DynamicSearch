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

        [HttpGet("{clientName}/{schemaName}/{entity}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Search([ModelBinder(typeof(QueryCommandModelBinder))] SearchClientQueryCommand searchClientQueryCommand)
            => Ok(await _mediator.Send(searchClientQueryCommand));
    }
}