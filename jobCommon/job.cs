using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace xingyi.job
{
    public class Job
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Owner { get; set; }
        public List<SectionTemplate> Sections { get; set; }

        // Navigation Property
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public List<JobSectionTemplate> JobSectionTemplates { get; set; } = new List<JobSectionTemplate>();
    }
    public class JobSectionTemplate
    {
        public Guid JobId { get; set; }
        public Job Job { get; set; }

        public Guid SectionTemplateId { get; set; }
        public SectionTemplate SectionTemplate { get; set; }
    }


    public class Application
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid JobId { get; set; }

        [Required]
        [EmailAddress]
        public string Candidate { get; set; }

        public string DetailedComments { get; set; }

        // Navigation Properties
        public Job Job { get; set; }
        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }

    public class SectionTemplate
    {
        [Key]
        public Guid Id { get; set; }

        public Guid JobId { get; set; }

        public bool CanEditWho { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public List<Question> Questions { get; set; }
        public List<SectionTemplateQuestion> SectionTemplateQuestions { get; set; }
        public List<JobSectionTemplate> JobSectionTemplates { get; set; }
    }

    public class Section
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid SectionTemplateId { get; set; }


        public string Who { get; set; }


        public string Comments { get; set; }

        public bool Finished { get; set; }

        // Navigation Properties
        public SectionTemplate SectionTemplate { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();


    }
    public class SectionTemplateQuestion
    {
        public Guid SectionTemplateId { get; set; }
        public SectionTemplate SectionTemplate { get; set; }

        public Guid QuestionId { get; set; }
        public Question Question { get; set; }
    }
    public class Question
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool ScoreOutOfTen { get; set; }

        public bool Singleline { get; set; }

        public List<SectionTemplateQuestion> SectionTemplateQuestions { get; set; }
        public Question()
        {
            SectionTemplateQuestions = new List<SectionTemplateQuestion>();
        }
    }

    public class Answer
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        public Guid SectionId { get; set; }
        [Required]
        public Guid QuestionId { get; set; }

        public string AnswerText { get; set; }

        public int Score { get; set; }

        // Navigation Property
        public Question Question { get; set; }
    }
}
