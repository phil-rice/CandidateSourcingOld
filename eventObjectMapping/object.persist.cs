using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using xingyi.cas.client;
using xingyi.cas.common;
using xingyi.common;
using xingyi.events;
using xingyi.relationships;

namespace xingyi.erm
{
    public record EntityAndEvents(Entity entity, List<Event> events);
    public record UpdateEventsResult(List<EntityAndEvents> entities, List<ContentItem> items, List<ListDiffResult<Relationship>> rs)
   ;

    public interface IPersistUpdateEventsResult
    {
        Task persist(UpdateEventsResult result);
    }
    public class PersistUpdateEventsResult : IPersistUpdateEventsResult
    {
        readonly ICasAdder casAdder;
        readonly IEventStoreAdder esAdder;
        readonly IRelationshipUpdater relUpdater;

        public async Task persist(UpdateEventsResult result)
        {
            foreach (var ci in result.items) await casAdder.AddItemAsync(ci.Namespace, ci.Data, ci.Namespace);
            foreach (var (entity, events) in result.entities)
                foreach (var ev in events) await esAdder.addAsync(entity.nameSpace, entity.name, ev);
            foreach (var r in result.rs) await IRelationshipUpdater.process(relUpdater, r);
        }
    }

    public interface IWorkOutHowToPersistCasEvent
    {
        Task<UpdateEventsResult> persist(EventCasObjDefn defn, Entity e, Dictionary<string, object> dict);

        public static Task<UpdateEventsResult> workout<T>(IWorkOutHowToPersistCasEvent workout, EventCasObjDefn defn, Entity e, T t)
        {
            var json = JsonSerializer.Serialize<T>(t);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return workout.persist(defn, e, dict);
        }
    }

    public class PersistEventCasObjDefn : IWorkOutHowToPersistCasEvent
    {
        readonly IPersistRelationships persistRelationships;
        readonly IShaCodec shaCodec;
        readonly ICasGetter getter;

        public async Task<UpdateEventsResult> persist(EventCasObjDefn defn, Entity e, Dictionary<string, object> dict)
        {
            var json = JsonSerializer.Serialize(defn);
            var contentItem = ContentItem.from(shaCodec, e.nameSpace, json);
            var rels = await persistRelationships.update(defn.relationships, e, dict);
            var existing = await getter.GetItemAsync(e.nameSpace, contentItem.SHA);
            if (existing == null)
            {
                var ev = new SetToCasEvent(e.nameSpace, contentItem.SHA);
                return new UpdateEventsResult(
                    new List<EntityAndEvents> {
                    new EntityAndEvents(e, new List<Event> { ev})},
                        new List<ContentItem> { contentItem },
                        new List<ListDiffResult<Relationship>> { rels });
            }
            else
            {
                return new UpdateEventsResult(
                    new List<EntityAndEvents>(),
                    new List<ContentItem>(),
                    new List<ListDiffResult<Relationship>> { rels });
            }
        }
    }


    public interface IPersistRelationships
    {
        Task<ListDiffResult<Relationship>> update(List<IRelationshipDefn> relations, Entity e, Dictionary<string, object> dict);
    }

    public class PersistRelationships : IPersistRelationships
    {
        readonly IRelationshipFinder finder;

        public PersistRelationships(IRelationshipFinder finder)
        {
            this.finder = finder;
        }

        public async Task<ListDiffResult<Relationship>> update(List<IRelationshipDefn> relations, Entity e, Dictionary<string, object> dict)
        {
            var existing = await finder.findAll(e);
            var newRels = IRelationshipDefn.findAllRelationshipsIn(relations, e, dict);
            var result = Lists.GetListDifferences(existing, newRels);
            return result;
        }
    }


}
