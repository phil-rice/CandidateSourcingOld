using xingyi.cas.client;
using xingyi.cas.common;
using xingyi.common;
using xingyi.erm;
using xingyi.events.client;
using xingyi.relationships;

namespace xingyi.job
{
    using static JobDefns;
    public interface IJobFinder
    {
        Task<Job> load(string jobId);
    }
    public interface IJobSaver
    {
        Task<string> save(Job job);
    }
    public class JobSaver : IJobSaver
    {
        readonly ICasAdder adder;
        async public Task<string> save(Job job)
        {
            var data = Json.getBytes(job);
            return await adder.AddItemAsync(jobNs, data, "application/json");
        }
    }
    public interface IApplicationFinder
    {
        Task<List<Application>> forJob(string jobId);
        Task<List<Application>> forCandidate(string email);
        Task<List<Application>> toFillIn(string email);
    }
    public class ApplicationFinder : IApplicationFinder
    {
        readonly IRelationshipFinder finder;
        readonly IProcessedEventsGetter getter;

        async public Task<List<Application>> forCandidate(string email)
        {
            var entity = new Entity(DefaultEntityToJson.literalStore, emailNs, email);
            var rels = await finder.findByObjectAnd(entity, JobDefns.forCandidate);
            return await Relationship.fromSubjects<Application>(getter, rels);
        }

        public async Task<List<Application>> forJob(string jobId)
        {
            var entity = new Entity(DefaultEntityToJson.casStore, jobNs, jobId);
            var rels = await finder.findByObjectAnd(entity, JobDefns.forCandidate);
            return await Relationship.fromSubjects<Application>(getter, rels);
        }

        async public Task<List<Application>> toFillIn(string email)
        {
            var entity = new Entity(DefaultEntityToJson.literalStore, emailNs, email);
            var rels = await finder.findByObjectAnd(entity, JobDefns.fillsIn);
            return await Relationship.fromSubjects<Application>(getter, rels);
        }
    }
    public class JobFinder : IJobFinder
    {
        readonly ICasObjGetter getter;
        public async Task<Job> load(string jobId)
        {
            var result = await getter.GetObjAsync<Job>(jobNs, jobId);
            return result;

        }
    }
    public interface IApplicationSaver
    {
        Task<Entity> save(Application application);
    }
    public class ApplicationSaver : IApplicationSaver
    {
        readonly IWorkOutHowToPersistCasEvent workout;
        readonly IIdGenerator generator;
        readonly IPersistUpdateEventsResult persister;

        public async Task<Entity> save(Application application)
        {
            var entity = new Entity(DefaultEntityToJson.eventStore, applicationNs, await generator.GenerateId(applicationNs));
            var workedOut = await IWorkOutHowToPersistCasEvent.workout(workout, JobDefns.ApplicationDefn, entity, application);
            await persister.persist(workedOut);
            return entity;
        }
    }

}
