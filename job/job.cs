
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
        public string id;
        public string title { get; set; }
        public string description { get; set; }
        public string owner { get; set; }
        public List<Section> Sections { get; set; }
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
        public string Id { get; set; }
        /// <summary>
        /// This will be a UUID
        /// </summary>
        public string JobSha { get; set; }

        /// <summary>
        /// Candidates email
        /// </summary>
        public string Candidate { get; set; }
        public List<Section> Sections { get; set; }

        /// <summary>
        /// Filled in as a summary of the person
        /// </summary>
        public string DetailedComments { get; set; }
    }


    [ToString, Equals(DoNotAddEqualityOperators = true)]
    public class SectionTemplate
    {
        /// <summary>
        /// When this is a candidate section, you can't change the person filling it in
        /// 
        /// For interviews it's OK to change it
        /// </summary>
        public bool CanEditWho { get; set; }
        /// <summary>
        /// The email of the person who can fill in the data. Typically the candidate or interviewer
        /// </summary>
        public string Who { get; set; }
        /// <summary>
        /// Display name
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Display details of the purpose of this section
        /// </summary>
        public string Description { get; set; }
        public List<Question> Questions { get; set; }

       public Section asSection(string id)
        {
            return new Section
            {
                Id = id,
                CanEditWho = CanEditWho,
                Who = Who,
                Title = Title,
                Description = Description,
                Comments = "",
                Finished = false,
                Questions = Questions.Select(q => q.asAnswer()).ToList(),
            };


        }
    }

    public class Section : SectionTemplate
    {
        /// <summary>
        /// This is the name of the Section in the event store. Generated as a UUID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The person filling it in can make some general comments
        /// </summary>
        public string Comments { get; set; }
        public bool Finished { get; set; }
        public List<Answer> Questions { get; set; }
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

        public Answer asAnswer()
        {
            return new Answer { Title = Title, Description = Description, scoreOutOfTen = scoreOutOfTen, answer = "", score = 0 };
          }
    }

    public class Answer : Question
    {
        public string answer { get; set; }
        public int score { get; set; }
    }

}
