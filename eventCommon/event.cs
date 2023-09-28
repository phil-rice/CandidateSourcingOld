namespace xingyi.events
{
    using Newtonsoft.Json;
    using System;
    using System.Runtime.Serialization;
    using System.Text.Json;
    using xingyi.cas.client;
    using Json = Dictionary<string, object>;
    public interface Event
    {

        static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        static List<Event> jsonToList(string json)
        {
            return JsonConvert.DeserializeObject<List<Event>>(json, settings);
        }
        static string listToJson(List<Event> events)
        {
            return JsonConvert.SerializeObject(events, settings);
        }
        static string eventToJson(Event e)
        {
            return JsonConvert.SerializeObject(e, settings);
        }
        static Event jsonToEvent(String json)
        {
            return JsonConvert.DeserializeObject<Event>(json, settings);

        }

    }

    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class SetToCasEvent : Event
    {
        public string nameSpace { get; }
        public string sha { get; }

        public SetToCasEvent(string nameSpace, string sha)
        {

            this.nameSpace = nameSpace;
            this.sha = sha;
        }
    }

    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class SetFieldToValueEvent : Event
    {
        public string fieldName { get; }
        public object value { get; }

        public SetFieldToValueEvent(string fieldName, object value)
        {
            this.fieldName = fieldName;
            this.value = value;
        }

    }

    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class SetFieldToCasEvent : Event
    {
        public string fieldName { get; }
        public string nameSpace { get; }
        public string sha { get; }

        public SetFieldToCasEvent(string fieldName, string nameSpace, string sha)
        {
            this.fieldName = fieldName;
            this.nameSpace = nameSpace;
            this.sha = sha;
        }
    }
}