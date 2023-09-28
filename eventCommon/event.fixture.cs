
using xingyi.events;
namespace xingi.events.tests
{
    using Json = Dictionary<string, object>;
    public static class EventFixture
    {
        public static Event ev1 = new SetToCasEvent("namespace1", "sha1");
        public static Event ev2 = new SetFieldToValueEvent("field1", "value1");
        public static Event ev3 = new SetFieldToCasEvent("field2", "namespace2", "sha2");
        public static List<Event> ev12 = new List<Event> { ev1, ev2 };
        public static List<Event> ev123 = new List<Event> { ev1, ev2, ev3 };

        public static Json processed12 = IEventExecutor<Json>.ExecuteEvents(new DebugJsonEventExecutor(), ev12).GetAwaiter().GetResult();
        public static Json processed123 = IEventExecutor<Json>.ExecuteEvents(new DebugJsonEventExecutor(), ev123).GetAwaiter().GetResult();
    }
}
