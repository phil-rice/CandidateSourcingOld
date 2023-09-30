using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using xingyi.cas.client;
using xingyi.cas.common;
using xingyi.events;
using xingyi.events.client;

namespace xingyi.relationships
{
    using Json = Dictionary<string, object>;

    public interface IEntityToJson
    {
        Task<T> GetJsonAsync<T>(Entity entity);
    }
    public interface IRelationshipToJson
    {
        Task<List<T>> findBySubject<T>(Entity subject);
        Task<List<T>> findBySubjectAnd<T>(Entity subject, Relation relation);
        Task<List<T>> findByObject<T>(Entity obj);
        Task<List<T>> findByObjectAnd<T>(Entity obj, Relation relation);
    }

    public class DefaultEntityToJson : IEntityToJson
    {
        public static string eventStore = "events";
        public static string casStore = "cas";
        public static string literalStore = "literal";

        readonly ICasGetter casGetter;
        readonly IProcessedEventsGetter eventsGetter;

        public DefaultEntityToJson(ICasGetter casGetter, IProcessedEventsGetter eventsGetter)
        {
            this.casGetter = casGetter;
            this.eventsGetter = eventsGetter;
        }

        async public Task<T> GetJsonAsync<T>(Entity entity)
        {
            if (entity.store == literalStore) return JsonSerializer.Deserialize<T>('"'+entity.name + '"');
            if (entity.store == casStore) return await fromCas<T>(entity);
            if (entity.store == eventStore) return await fromEvent<T>(entity);
            throw new Exception($"Unknown store {entity}");
        }

        async private Task<T> fromEvent<T>(Entity entity)
        {
            var json = await eventsGetter.GetProcessedEventsAsync(entity.nameSpace, entity.name);
            string jsonString = JsonSerializer.Serialize(json);
            T obj = JsonSerializer.Deserialize<T>(jsonString);
            return obj;
        }

        async private Task<T> fromCas<T>(Entity entity)
        {
            var ci = await casGetter.GetItemAsync(entity.nameSpace, entity.name);
            if (ci == null) throw new Exception($"Entity not found {entity}");
            if (!ci.MimeType.Contains("json")) throw new Exception($"Have cas that isn't json for {entity}\n{ci}");
            string jsonString = System.Text.Encoding.UTF8.GetString(ci.Data);
            var result = JsonSerializer.Deserialize<T>(jsonString);
            return result;
        }
    }
    public class RelationshipToJson : IRelationshipToJson
    {

        readonly IRelationshipFinder finder;
        readonly IEntityToJson toJson;

        public Task<List<T>> findByObject<T>(Entity obj)
        {
            return from<T>(finder.findByObject(obj), rel => rel.subject());
        }

        public Task<List<T>> findByObjectAnd<T>(Entity obj, Relation relation)
        {
            return from<T>(finder.findByObjectAnd(obj, relation), rel => rel.subject());
        }

        public Task<List<T>> findBySubject<T>(Entity subject)
        {
            return from<T>(finder.findBySubject(subject), rel => rel.obj());
        }

        public Task<List<T>> findBySubjectAnd<T>(Entity subject, Relation relation)
        {
            return from<T>(finder.findBySubjectAnd(subject, relation), rel => rel.obj());
        }


        async Task<List<T>> from<T>(Task<List<Relationship>> taskRels, Func<Relationship, Entity> fn)
        {
            var rels = await taskRels;
            var result = await Task.WhenAll(rels.Select(rel => toJson.GetJsonAsync<T>(fn(rel))));
            return result.ToList();

        }

    }
}