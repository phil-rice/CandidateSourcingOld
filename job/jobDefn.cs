using xingyi.erm;
using xingyi.events;
using xingyi.relationships;
using xinyi.job;

namespace xingyi.job
{
    using static JobRepository;
    public static class JobDefns
    {
        public static IObjectDefn ApplicationDefn = new EventCasObjDefn(applicationNs, new List<IRelationshipDefn>());
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
