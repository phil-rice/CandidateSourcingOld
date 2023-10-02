using System.Collections.Generic;
using System.Text.Json;
using NUnit.Framework;
using xingyi.common;

namespace xingyi.job.Tests
{
    public static class JobDeserialisationFixture
    {
        public readonly static Question question1 = new Question
        {
            Id = Guids.from("question1Id"),
            Title = "Question1Title",
            Description = "Question1Description",
            Singleline = false,
            ScoreOutOfTen = true
        };
        public static readonly Answer answer1 = new Answer
        {
            Id = Guids.from("AnswerId1"),
            QuestionId = question1.Id,
            Question = question1,
            AnswerText = "some answer1",
            Score = 10
        };
        public static readonly Answer answer2 = new Answer
        {
            Id = Guids.from("AnswerId2"),
            QuestionId = question2.Id,
            Question = question2,
            AnswerText = "some answer2",
            Score = 8
        };
        public static readonly Question question2 = new Question
        {
            Id = Guids.from("question2Id"),
            Title = "Question2Title",
            Description = "Question2Description",
            Singleline = true,
            ScoreOutOfTen = true
        };
        private static SectionTemplate st1 = new SectionTemplate
        {
            Id = Guids.from( "SectionID"),
            CanEditWho = true,
            Title = "SectionTitle",
            Description = "SectionDescription",
            Questions = new List<Question> { question1, question2 }
        };
        public static readonly Section sec1 = new Section
        {
            Id = Guids.from("Section1ID"),
            SectionTemplate = st1,
            SectionTemplateId = st1.Id,
            Who = "Section1InterviewerEmail@example.com",
            Comments = "Section1Comments",
            Finished = false,
            Answers = new List<Answer> { answer1, answer2 }
        };
        private static Job job = new Job
        {
            Id =Guids.from("JobShaID"),
            Title = "JobTitle",
            Description = "JobDescription",
            Owner = "JobOwner",
            Sections = new List<SectionTemplate> { st1 }
        };

        public static readonly Application ApplicationConstant = new Application
        {
            Id = Guids.from("ApplicationID"),
            JobId = job.Id,
            Job = job,
            Candidate = "ApplicationCandidateEmail@example.com",
            DetailedComments = "ApplicationDetailedComments",
            Sections = new List<Section> { sec1 }
        };

        public static readonly string JobJson = @"{
    ""Id"": ""JobShaID"",
    ""Title"": ""JobTitle"",
    ""Description"": ""JobDescription"",
    ""Owner"": ""JobOwner"",
    ""Sections"": [
        {
            ""Id"": ""SectionID"",
            ""CanEditWho"": true,
            ""Title"": ""SectionTitle"",
            ""Description"": ""SectionDescription"",
            ""Questions"": [
                {
                    ""Id"": ""question1Id"",
                    ""Title"": ""Question1Title"",
                    ""Description"": ""Question1Description"",
                    ""Singleline"": false,
                    ""ScoreOutOfTen"": true
                },
                {
                    ""Id"": ""question2Id"",
                    ""Title"": ""Question2Title"",
                    ""Description"": ""Question2Description"",
                    ""Singleline"": true,
                    ""ScoreOutOfTen"": true
                }
            ]
        }
    ]
}";

        public static readonly string ApplicationJson = @"{
    ""Id"": ""ApplicationID"",
    ""JobSha"": ""JobShaID"",
    ""Candidate"": ""ApplicationCandidateEmail@example.com"",
    ""DetailedComments"": ""ApplicationDetailedComments"",
    ""Sections"": [
        {
            ""Id"": ""Section1ID"",
            ""SectionTemplateId"": ""SectionID"",
            ""Who"": ""Section1InterviewerEmail@example.com"",
            ""Comments"": ""Section1Comments"",
            ""Finished"": false,
            ""Answers"": [
                {
                    ""Id"": ""AnswerId1"",
                    ""QuestionId"": ""question1Id"",
                    ""AnswerText"": ""some answer1"",
                    ""Score"": 10
                },
                {
                    ""Id"": ""AnswerId2"",
                    ""QuestionId"": ""question2Id"",
                    ""AnswerText"": ""some answer2"",
                    ""Score"": 8
                }
            ]
        }
    ]
}";
    }
}
