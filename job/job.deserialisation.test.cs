using NUnit.Framework;
using System.Text.Json;

namespace xingyi.job

{
    public class JobDeserialisationTest
    {

        public static readonly Job job = new Job
        {
            id = "JobID",
            title = "JobTitle",
            description = "JobDescription",
            owner = "JobOwner",
            Sections = new List<Section>
        {
            new Section
            {
                Id = "SectionID",
                CanEditWho = true,
                Who = "SectionWho",
                Title = "SectionTitle",
                Description = "SectionDescription",
                Comments = "SectionComments",
                Finished = true,
                Questions = new List<Answer>
                {
                    new Answer
                    {
                        Title = "AnswerTitle",
                        Description = "AnswerDescription",
                        scoreOutOfTen = true,
                        answer = "AnswerAnswer",
                        score = 10
                    }
                }
            }
        }
        };

        public static readonly Application ApplicationConstant = new Application
        {
            Id = "ApplicationID",
            JobSha = "JobShaID",
            Candidate = "ApplicationCandidate",
            DetailedComments = "ApplicationDetailedComments",
            Sections = job.Sections
        };


        public const string jobJson = @"{
    ""id"": ""JobID"",
    ""title"": ""JobTitle"",
    ""description"": ""JobDescription"",
    ""owner"": ""JobOwner"",
    ""Sections"": [
        {
            ""Id"": ""SectionID"",
            ""CanEditWho"": true,
            ""Who"": ""SectionWho"",
            ""Title"": ""SectionTitle"",
            ""Description"": ""SectionDescription"",
            ""Comments"": ""SectionComments"",
            ""Finished"": true,
            ""Questions"": [
                {
                    ""Title"": ""AnswerTitle"",
                    ""Description"": ""AnswerDescription"",
                    ""scoreOutOfTen"": true,
                    ""answer"": ""AnswerAnswer"",
                    ""score"": 10
                }
            ]
        }
    ]
}";

        [Test]
        public void TestCanConvertJsonToJob()
        {
            Job convertedJob = JsonSerializer.Deserialize<Job>(jobJson);
            Assert.AreEqual(job.ToString(), convertedJob.ToString());
        }
    }
}