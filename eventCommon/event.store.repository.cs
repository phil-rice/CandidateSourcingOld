using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace xingyi.events
{
    public interface IEventStoreGetter
    {
        Task<List<Event>> getAsync(string nameSpace, string name);
    }
    public interface IEventStoreAdder
    {
        Task<List<Event>> addAsync(string nameSpace, string name, Event e);
    }

    public class EventStoreRepository : IEventStoreGetter, IEventStoreAdder
    {
        readonly EventStoreDbContext dbContext;

        public EventStoreRepository(EventStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async public Task<List<Event>> addAsync(string nameSpace, string name, Event e)
        {
            var storedEvent = await dbContext.StoredEvents.FindAsync(nameSpace, name);
            if (storedEvent == null)
            {
                storedEvent = new StoredEvent { Namespace = nameSpace, Name = name };
                dbContext.StoredEvents.Add(storedEvent);
            }
            var json = storedEvent?.JsonEvents ?? "[]";
            var events = Event.jsonToList(json);
            events.Add(e);
            var newJson = Event.listToJson(events);
            storedEvent.JsonEvents = newJson;
            dbContext.SaveChanges();
            return events;
        }

        async public Task<List<Event>> getAsync(string nameSpace, string name)
        {
            var storedEvent = await dbContext.StoredEvents.FindAsync(nameSpace, name);
            var json = storedEvent?.JsonEvents ?? "[]";
            return Event.jsonToList(json);
        }
    }
}
