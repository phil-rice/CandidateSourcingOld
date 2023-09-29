
namespace xingyi.job
{

    /// <summary>
    /// A job is a specific type of vacancy.
    /// 
    ///It defines the process (how many sections) and the questions in each section
    ///
    /// While the job is in the event store, any given application follows the job given at the time the application was in place
    /// </summary>
    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class Job
    {
        /// <summary>
        /// The name of the job in the event store
        /// </summary>
        string id;
        string title { get; set; }
        string description { get; set; }
        string owner { get; set; }
        public Dictionary<string, SectionTemplate> Rounds { get; set; }
    }

    /// <summary>
    /// An application is the information about a specific candidate applying for a specific job
    /// </summary>
    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class Application
    {

        /// <summary>
        /// The name of the application in the event store. Generated as a UUID
        /// </summary>
        string id { get; set; }
        /// <summary>
        /// This will be a UUID
        /// </summary>
        string jobSha { get; set; }

        /// <summary>
        /// Candidates email
        /// </summary>
        string candidate { get; set; }
        Dictionary<string, Section> nameToSectionEvent { get; set; }

        /// <summary>
        /// Filled in as a summary of the person
        /// </summary>
        string detailedComments { get; set; }
    }


    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class SectionTemplate
    {
        /// <summary>
        /// When this is a candidate section, you can't change the person filling it in
        /// 
        /// For interviews it's OK to change it
        /// </summary>
        public bool canEditWho { get; set; }
        /// <summary>
        /// The email of the person who can fill in the data. Typically the candidate or interviewer
        /// </summary>
        public string who { get; set; }
        /// <summary>
        /// Display name
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Display details of the purpose of this section
        /// </summary>
        public string description { get; set; }
        public List<Question> questions { get; set; }
    }

    public class Section : SectionTemplate
    {
        /// <summary>
        /// This is the name of the Section in the event store. Generated as a UUID
        /// </summary>
        public string sectionId { get; set; }
        /// <summary>
        /// The person filling it in can make some general comments
        /// </summary>
        public string comments { get; set; }
        public bool finished { get; set; }
        public List<Answer> questions { get; set; }
    }

    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class Question
    {
        /// <summary>
        /// The text for the question
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Any supporting information 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// As well as a string, do we fill in a score out of 10?
        /// </summary>
        public bool scoreOutOfTen { get; set; }

    }

    public class Answer : Question
    {
        public string answer { get; set; }
        public int score { get; set; }
    }

}
