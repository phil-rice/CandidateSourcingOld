using xingyi.cas.common;
using xingyi.common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using xingyi.common.validator;
using Microsoft.AspNetCore.Mvc;
using xingyi.events;
using xingyi.cas.client;

namespace eventApi
{
    [ApiController]
    [Route("events")]
    public class EventsController : ControllerBase
    {

        private readonly IEventStoreGetter getter;
        private readonly IEventStoreAdder adder;
        private readonly IEventExecutor<Dictionary<string, object>> eventExecutor;



        [HttpGet("{nameSpace}/{name}")]
        public async Task<IActionResult> GetEvents(string nameSpace, string name)
        {
            var events = await getter.getAsync(nameSpace, name);
            if (events.Count == 0) return NotFound();
            var result = await IEventExecutor<Dictionary<string, object>>.ExecuteEvents(eventExecutor, events);
            return Ok(result);
        }
        [HttpPost("{nameSpace}/{name}")]
        public async Task<IActionResult> AddEvent(string nameSpace, string name, [FromBody] string body)
        {
            var e = Event.jsonToEvent(body);
            var events = await adder.addAsync(nameSpace, name, e);
            var result = await IEventExecutor<Dictionary<string, object>>.ExecuteEvents(eventExecutor, events);
            return Ok(result);
        }

        [HttpGet("{nameSpace}/{name}/raw")]
        public async Task<IActionResult> GetRawEvents(string nameSpace, string name)
        {
            var events = await getter.getAsync(nameSpace, name);
            if (events.Count == 0) return NotFound();
            var json = Event.listToJson(events);
            return Content(json, "application/json");
        }
    }

}
