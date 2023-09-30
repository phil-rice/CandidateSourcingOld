using xingyi.cas.client;
using xingyi.events;
using xingyi.events.client;
using xingyi.job;
using xingyi.relationships;
using System.Linq;
using xingyi.common;
using System.Reflection.Emit;

namespace xinyi.job
{
    public interface IJobGetter
    {
        Task<Job> getAsync(string jobId);
    }

    public interface IJobSaver
    {
        Task SaveAsync(Job job);
    }

    public interface IApplicationMaker
    {
        Task<Application> create(Job job, string candidateEmail);
    }
    public interface IApplicationGetter
    {
        Task<Job> getAsync(string id);
    }
    public interface IApplicationSaver
    {
        Task updateDetails(string id, string details);
    }

    public interface ISectionGetter
    {
        Task<Section> getSectionAsync(string id);
    }
    public interface ISectionSaver
    {
        Task saveSection(Section section);
    }

    public interface IJobFinder
    {
        Task<List<Job>> jobs(string ownerEmail);
    }

    public interface IApplicationFinder
    {
        Task<List<Application>> getApplications(string jobId);
    }

    public record ApplicationAndSection(Application application, Section section);

    public interface ISectionFinder
    {
        Task<List<ApplicationAndSection>> getForMe(string jobId, string email);

    }

    public class JobRepository
    //: IJobFinder, IJobSaver, IApplicationMaker, IApplicationGetter, IApplicationSaver, ISectionGetter, ISectionSaver, ISectionFinder
    {

        public static string jobNs = "job";
        public static string applicationNs = "application";
        public static string sectionTemplateNs = "sectionTemplate";
        public static string sectionNs = "section";
        public static string answersNs = "answers";
        public static string emailNs = "email";
        public static Relation filledInBy = new Relation("job", "filledInBy");
        public static Relation ownedBy = new Relation("job", "ownedBy");
    }
    //    readonly IRelationshipToJson relToJson;
    //    readonly IEntityToJson entityToJson;
    //    readonly IRelationshipUpdater relationshipUpdater;

    //    readonly IEventStoreGetter eventsGetter;
    //    readonly IEventStoreAdder eventsAdder;

    //    readonly ISetToCas setToCas;
    //    readonly ISetFieldToValue setFieldToValue;
    //    readonly ISetFieldToCasValue setFieldToCasValue;

    //    readonly ICasAdder casAdder;
    //    readonly IIdGenerator idGenerator;

    //}

    //public Task<Job> getAsync(string id)
    //{
    //    return entityToJson.GetJsonAsync<Job>(new Entity(DefaultEntityToJson.eventStore, applicationNs, id));
    //}

    //async public Task<List<Job>> jobs(string ownerEmail)
    //{
    //    return await relToJson.findByObjectAnd<Job>(new Entity(emailStore, emailNs, ownerEmail), ownedBy);
    //}

    //public Task SaveAsync(Job job)
    //{
    //    //A biggy. Does all the work
    //    //Splits into sections. If the sections haven't changed then nothing, otherwise update it
    //    //if the title, description changes then need to change it
    //    //can't change owner or id
    //    throw new NotImplementedException();
    //}

}

