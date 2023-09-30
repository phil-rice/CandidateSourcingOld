using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xingyi.common;
using xingyi.relationships;

namespace xingyi.erm
{

    public interface IPersistEventCasObjDefn
    {
        Task<Entity> persist(EventCasObjDefn defn, Entity e, Dictionary<string, object> dict);
    }




    public interface IPersistRelationships
    {
        Task<List<Relationship>> update(List<IRelationshipDefn> relations, Entity e, Dictionary<string, object> dict);
    }
    public class PersistRelationships : IPersistRelationships
    {
        readonly IRelationshipUpdater updater;
        readonly IRelationshipFinder finder;

        public PersistRelationships(IRelationshipUpdater updater, IRelationshipFinder finder)
        {
            this.updater = updater;
            this.finder = finder;
        }

        public async Task<List<Relationship>> update(List<IRelationshipDefn> relations, Entity e, Dictionary<string, object> dict)
        {
            var existing = await finder.findAll(e);
            var newRels = IRelationshipDefn.findAllRelationshipsIn(relations, e, dict);
            var (Added, Removed) = Lists.GetListDifferences(existing, newRels);
            foreach (var r in Removed) await updater.delete(r);
            foreach (var r in Added) await updater.set(r);
            return newRels;
        }
    }


}
