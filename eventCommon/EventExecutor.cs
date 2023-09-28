
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xingyi.cas.client;
using xingyi.events;

namespace xingyi.events
{
    using Json = Dictionary<string, object>;

    public record RawAndEvents<T>(List<Event> events, T value)
    {
        async public static Task<RawAndEvents<T>> make<T>(IEventExecutor<T> executor, List<Event> events)
        {
            return new RawAndEvents<T>(events, await IEventExecutor<T>.ExecuteEvents(executor, events));
        }
    }


    public interface IEventExecutor<T>
    {
        T zero();
        Task<T> SetToCas(SetToCasEvent e, T t);
        Task<T> SetFieldToValue(SetFieldToValueEvent e, T t);
        Task<T> SetFieldToCas(SetFieldToCasEvent e, T t);

        static public async Task<T> ExecuteEvents(IEventExecutor<T> executor, List<Event> events)
        {
            T currentState = executor.zero();

            foreach (var evt in events)
            {
                switch (evt)
                {
                    case SetToCasEvent e:
                        currentState = await executor.SetToCas(e, currentState);
                        break;
                    case SetFieldToValueEvent e:
                        currentState = await executor.SetFieldToValue(e, currentState);
                        break;
                    case SetFieldToCasEvent e:
                        currentState = await executor.SetFieldToCas(e, currentState);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown event type." + evt);
                }
            }

            return currentState;
        }
    }


    public class JsonEventExecutor : IEventExecutor<Json>
    {
        readonly ICasJsonGetter getter;

        public JsonEventExecutor(ICasJsonGetter getter)
        {
            this.getter = getter;
        }

        public Json zero()
        {
            return new Dictionary<string, object>();
        }
        async public Task<Json> SetToCas(SetToCasEvent e, Json json)
        {
            return await getter.GetJsonAsync(e.nameSpace, e.sha);
        }
        async public Task<Json> SetFieldToValue(SetFieldToValueEvent e, Json json)
        {
            json[e.fieldName] = e.value;
            return json;
        }
        async public Task<Json> SetFieldToCas(SetFieldToCasEvent e, Json json)
        {
            json[e.fieldName] = await getter.GetJsonAsync(e.nameSpace, e.sha);
            return json;
        }

    }
}