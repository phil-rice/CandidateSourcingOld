using System.Text;
using System.Text.Json;
using xingyi.common.http;

namespace xingyi.events.client
{
    using Json = Dictionary<string, object>;
    public interface IRawEventsGetter
    {
        Task<List<Event>> GetEventsAsync(string nameSpace, string name);
    }

    public interface IProcessedEventsGetter
    {
        Task<Json> GetProcessedEventsAsync(string nameSpace, string name);
    }
    public interface IAddEvent
    {
        Task<Json> AddEvent(string nameSpace, string name, Event e);
    }

    public class EventClient : IRawEventsGetter, IProcessedEventsGetter, IAddEvent
    {
        private readonly IHttpClient httpClient;

        public EventClient(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        async public Task<Json> AddEvent(string nameSpace, string name, Event e)
        {
            var body = Encoding.UTF8.GetBytes(Event.eventToJson(e));
            var response = (await httpClient.post($"/events/{nameSpace}/{name}", "application/json", body)).valueOrError();
            return JsonSerializer.Deserialize<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
        }

        async public Task<List<Event>> GetEventsAsync(string nameSpace, string name)
        {
            var response = (await httpClient.get($"/events/{nameSpace}/{name}/raw")).valueOrError();
            var json = response==null?"[]":await response.Content.ReadAsStringAsync();
            return response == null ? new List<Event>() : Event.jsonToList(json);
        }

        async public Task<Json> GetProcessedEventsAsync(string nameSpace, string name)
        {
            var response = (await httpClient.get($"/events/{nameSpace}/{name}")).valueOrError();
            return JsonSerializer.Deserialize<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
        }
    }

}