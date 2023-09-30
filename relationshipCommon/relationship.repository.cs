using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace xingyi.relationships
{
    public interface IRelationshipFinder
    {
        Task<List<Relationship>> findBySubject(Entity subject);
        Task<List<Relationship>> findBySubjectAnd(Entity subject, Relation relation);
        Task<Relationship?> findBySubjectAndObject(Entity subject, Relation relation, Entity obj);
        Task<List<Relationship>> findByObject(Entity obj);
        Task<List<Relationship>> findByObjectAnd(Entity obj, Relation relation);
        Task<List<Relationship>> findAll(Entity entity);
    }

    public interface IRelationshipUpdater
    {
        Task set(Relationship relationship);
        Task delete(Relationship relationship);
        Task update(Relationship relationship);
    }

    public class RelationshipRepository : IRelationshipFinder, IRelationshipUpdater
    {
        readonly RelationshipDbContext context;

        public RelationshipRepository(RelationshipDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Relationship>> findBySubject(Entity subject)
        {
            return await context.Relationships
                .Where(r => r.SubjectStore == subject.store &&
                            r.SubjectNamespace == subject.nameSpace &&
                            r.SubjectName == subject.name)
                .ToListAsync();
        }

        public async Task<List<Relationship>> findBySubjectAnd(Entity subject, Relation relation)
        {
            return await context.Relationships
                .Where(r => r.SubjectStore == subject.store &&
                            r.SubjectNamespace == subject.nameSpace &&
                            r.SubjectName == subject.name &&
                            r.RelationshipNamespace == relation.nameSpace &&
                            r.RelationshipName == relation.name)
                .ToListAsync();
        }

        public async Task<Relationship?> findBySubjectAndObject(Entity subject, Relation relation, Entity obj)
        {
            return await context.Relationships
                .FirstOrDefaultAsync(r => r.SubjectStore == subject.store &&
                                          r.SubjectNamespace == subject.nameSpace &&
                                          r.SubjectName == subject.name &&
                                          r.RelationshipNamespace == relation.nameSpace &&
                                          r.RelationshipName == relation.name &&
                                          r.ObjectStore == obj.store &&
                                          r.ObjectNamespace == obj.nameSpace &&
                                          r.ObjectName == obj.name);
        }

        public async Task<List<Relationship>> findByObject(Entity obj)
        {
            return await context.Relationships
                .Where(r => r.ObjectStore == obj.store &&
                            r.ObjectNamespace == obj.nameSpace &&
                            r.ObjectName == obj.name)
                .ToListAsync();
        }

        public async Task<List<Relationship>> findByObjectAnd(Entity obj, Relation relation)
        {
            return await context.Relationships
                .Where(r => r.ObjectStore == obj.store &&
                            r.ObjectNamespace == obj.nameSpace &&
                            r.ObjectName == obj.name &&
                            r.RelationshipNamespace == relation.nameSpace &&
                            r.RelationshipName == relation.name)
                .ToListAsync();
        }

        public async Task<List<Relationship>> findAll(Entity entity)
        {
            return await context.Relationships
                    .Where(r => (r.ObjectStore == entity.store &&
                                r.ObjectNamespace == entity.nameSpace &&
                                r.ObjectName == entity.name)
                                ||
                                (r.SubjectStore == entity.store &&
                                          r.SubjectNamespace == entity.nameSpace &&
                                          r.SubjectName == entity.name))
                    .ToListAsync();
        }

        public async Task set(Relationship relationship)
        {
            if (relationship.Id != 0) throw new Exception($"Cannot set with relationship because already has id {relationship}");
            var existing = await this.findBySubjectAndObject(relationship.subject(), relationship.relation(), relationship.obj());
            if (existing == null)
            {
                context.Relationships.Add(relationship);
                await context.SaveChangesAsync();

            }
        }

        public async Task update(Relationship relationship)
        {
            context.Relationships.Update(relationship);
            await context.SaveChangesAsync();
        }

        public async Task delete(Relationship relationship)
        {
            context.Relationships.Remove(relationship);
            await context.SaveChangesAsync();
        }


    }
}
