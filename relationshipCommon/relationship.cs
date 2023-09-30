namespace xingyi.relationships
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public record Relation(string nameSpace, string name);
    public record Entity(string store, string nameSpace, string name);

    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class Relationship
    {
        public static Relationship from(Entity subject, Relation r, Entity obj)
        {
            return new Relationship
            {
                SubjectStore = subject.store,
                SubjectNamespace = subject.nameSpace,
                SubjectName = subject.name,
                RelationshipNamespace = r.nameSpace,
                RelationshipName = r.name,
                ObjectStore = obj.store,
                ObjectNamespace = obj.nameSpace,
                ObjectName = obj.name
            };
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [IgnoreDuringEquals]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string SubjectStore { get; set; }

        [Required]
        [MaxLength(100)]
        public string SubjectNamespace { get; set; }

        [Required]
        [MaxLength(100)]
        public string SubjectName { get; set; }

        [Required]
        [MaxLength(100)]
        public string RelationshipNamespace { get; set; }

        [Required]
        [MaxLength(100)]
        public string RelationshipName { get; set; }

        [Required]
        [MaxLength(100)]
        public string ObjectStore { get; set; }

        [Required]
        [MaxLength(100)]
        public string ObjectNamespace { get; set; }

        [Required]
        [MaxLength(100)]
        public string ObjectName { get; set; }

        public Entity subject() { return new Entity(SubjectStore, SubjectNamespace, SubjectName); }
        public Relation relation() { return new Relation(RelationshipNamespace, RelationshipName); }
        public Entity obj() { return new Entity(ObjectStore, ObjectNamespace, ObjectName); }



    }
}