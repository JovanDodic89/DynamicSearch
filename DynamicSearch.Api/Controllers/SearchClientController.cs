using DynamicSearch.Application.Searches.Queries.SearchClient;
using DynamicSearch.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;

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
        public async Task<ActionResult> Search([FromQuery] SearchClientQueryCommand searchClientQueryCommand)
        {
            var result = await _mediator.Send(searchClientQueryCommand);

            if (result == null || result.TotalCount == 0)
            {
                return NoContent();
            }

            return Ok(result);
        }
    }
}
