using xingyi.erm;
using xingyi.events;
using xingyi.relationships;

namespace xingyi.job
{

    public static class JobDefns
    {
        public static string jobNs = "job";
        public static string applicationNs = "application";
        public static string emailNs = "email";


        public static Relation forCandidate = new Relation("job", "applicationFor");
        public static Relation fillsIn = new Relation("job", "fillsin");
        public static Relation hasApplication = new Relation("job", "application");
        public static EventCasObjDefn ApplicationDefn = new EventCasObjDefn(applicationNs, new List<IRelationshipDefn> {
            new SubjectRelationDefn(DefaultEntityToJson.casStore, jobNs, "JobSha",hasApplication),
            new SubjectRelationDefn(DefaultEntityToJson.literalStore, emailNs, "Candidate",forCandidate),
            new ListRelationDefn("Who", new List<IRelationshipDefn> { new SubjectRelationDefn(DefaultEntityToJson.literalStore, emailNs,"Who",fillsIn) })});
        public static IObjectDefn JobDefn = new EventCasObjDefn(jobNs, new List<IRelationshipDefn>());



        //public static IObjectDefn SectionDefn =
        //    new AggregateObjDefn(sectionNs, "Id",
        //         new List<IFieldUpdateDefn>
        //         {
        //            new FieldValueUpdateDefn("Id"),
        //            new FieldValueUpdateDefn("Who"),
        //            new FieldValueUpdateDefn("comments"),
        //            new FieldValueUpdateDefn("finished"),
        //            new FieldValueStoreUpdateDefn("Questions",new CasObjDefn(DefaultEntityToJson.casStore)),
        //         }, new List<IRelationshipDefn>());

        //public static IObjectDefn ApplicationDefn =
        //      new AggregateObjDefn(applicationNs, "Id",
        //           new List<IFieldUpdateDefn> {
        //             new FieldValueUpdateDefn("Id"),
        //             new FieldValueUpdateDefn("DetailedComments"),
        //             new FieldListStoreUpdateDefn("Sections", SectionDefn) },
        //            new List<IRelationshipDefn>());

    }
}
