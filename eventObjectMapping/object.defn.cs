using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using xingyi.cas.common;
using xingyi.common;
using xingyi.events;
using xingyi.relationships;

namespace xingyi.erm
{
    public interface IObjectDefn
    {
        string store { get; }
        string nameSpace { get; }
        List<IRelationshipDefn> relationships { get; }
    }

    /// <summary>
    /// The data about the event is just stored in the cas. There is no 'splitting up' or 'field manipulation'. This is 'brute force' but quick to code up
    /// </summary>
    public record EventCasObjDefn(string nameSpace, List<IRelationshipDefn> relationships, string contentType = "application/json") : IObjectDefn
    {
        public string store => DefaultEntityToJson.eventStore;
    }
    /// <summary>
    /// When we save the object we just save it to the Cas. 
    /// </summary>
    public record CasObjDefn(string nameSpace, List<IRelationshipDefn> relationships, string contentType = "application/json") : IObjectDefn
    {
        public string store => DefaultEntityToJson.casStore;
    }
    /// <summary>
    /// Not implemented yet for our MVP. This is the potent bit in the story: we can split the object up, saving some of it in the CAS, some of it in events, some it as other entities
    /// </summary>
    public record AggregateObjDefn(string nameSpace, string nameField, List<IFieldUpdateDefn> fields, List<IRelationshipDefn> relationships) : IObjectDefn
    {
        public string store => DefaultEntityToJson.eventStore;
    }

    public interface IFieldUpdateDefn
    {
        string field { get; }
    }
    public record FieldValueUpdateDefn(string field) : IFieldUpdateDefn;
    public record FieldValueStoreUpdateDefn(string field, IObjectDefn defn) : IFieldUpdateDefn;
    public record FieldListStoreUpdateDefn(string field, IObjectDefn defn) : IFieldUpdateDefn;
    public record FieldDicStoreUpdateDefn(string field, IObjectDefn defn) : IFieldUpdateDefn;

    public record EventAndEvents(string nameSpace, string name, List<Event> events);
    public record EventsAndCas(List<EventAndEvents> es, List<ContentItem> cis);

    public interface IRelationshipDefn
    {
        public List<Relationship> findRelationsIn(Entity entity, Dictionary<string, object> dict);
        public static List<Relationship> findAllRelationshipsIn(List<IRelationshipDefn> relationshipDefns, Entity entity, Dictionary<string, object> dict)
        {
            return relationshipDefns.SelectMany(relDefn => relDefn.findRelationsIn(entity, dict)).ToList();
        }

    }



    public record ListRelationDefn(string listRelationshipName, List<IRelationshipDefn> relationship) : IRelationshipDefn
    {
        public List<Relationship> findRelationsIn(Entity entity, Dictionary<string, object> dict)
        {
            var obj = dict[listRelationshipName] ?? new List<Dictionary<string, object>>();
            if (!(obj is List<Dictionary<string, object>>)) throw new Exception($"Cannot get relationships from obj {obj.GetType()}\n{obj}");
            List<Dictionary<string, object>> list = obj as List<Dictionary<string, object>>;
            List<List<Relationship>> result = relationship.SelectMany(rel => list, (relation, newDict) => relation.findRelationsIn(entity, newDict)).ToList();
            return result.SelectMany(list => list).ToList();
        }
    }

    public record SubjectRelationDefn(string subjectStore, string subjectNamespace, string field, Relation Relation) : IRelationshipDefn
    {
        public List<Relationship> findRelationsIn(Entity entity, Dictionary<string, object> dict)
        {
            return new List<Relationship> { Relationship.from(new Entity(subjectStore, subjectNamespace, dict[field].ToString()), Relation, entity) };
        }
    }

    public record ObjectRelationDefn(string objStore, string objNamespace, string field, Relation Relation) : IRelationshipDefn
    {
        public List<Relationship> findRelationsIn(Entity entity, Dictionary<string, object> dict)
        {
            return new List<Relationship> { Relationship.from(entity, Relation, new Entity(objStore, objNamespace, dict[field].ToString())) };
        }
    }

}