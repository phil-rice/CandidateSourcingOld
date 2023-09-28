
namespace xingyi.job
{
    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class Job
    {
        public Dictionary<string, Round> Rounds { get; set; }

        public Job(Dictionary<string, Round> rounds)
        {
            Rounds = rounds;
        }
    }
    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class Round
    {
        public Dictionary<string, Section> Sections { get; set; }

        public Round(Dictionary<string, Section> sections)
        {
            Sections = sections;
        }
    }
    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class Section
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public Section(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }

}
