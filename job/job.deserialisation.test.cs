using NUnit.Framework;
using System.Text.Json;

namespace xingyi.job

{
    public class JobDeserialisationTests
    {
        Job job = new Job(new Dictionary<string, Round>
        {
            ["tp1"] = new Round(new Dictionary<string, Section>
            {
                ["section1"] = new Section("Title 1", "Description 1"),
                ["section2"] = new Section("Title 2", "Description 2"),
                ["section3"] = new Section("Title 3", "Description 3"),
                ["section4"] = new Section("Title 4", "Description 4")
            }),
            ["tp2"] = new Round(new Dictionary<string, Section>
            {
                ["section1"] = new Section("Title 5", "Description 5"),
                ["section2"] = new Section("Title 6", "Description 6"),
                ["section3"] = new Section("Title 7", "Description 7"),
                ["section4"] = new Section("Title 8", "Description 8")
            }),
            ["hr"] = new Round(new Dictionary<string, Section>
            {
                ["section1"] = new Section("Title 9", "Description 9"),
                ["section2"] = new Section("Title 10", "Description 10"),
                ["section3"] = new Section("Title 11", "Description 11"),
                ["section4"] = new Section("Title 12", "Description 12")
            })
        });



        public static string json = @"{
        ""Rounds"": {
            ""tp1"": {
                ""Sections"": {
                    ""section1"": {""Title"": ""Title 1"",""Description"": ""Description 1"" },
                    ""section2"": {""Title"": ""Title 2"",""Description"": ""Description 2"" },
                    ""section3"": {""Title"": ""Title 3"",""Description"": ""Description 3"" },
                    ""section4"": {""Title"": ""Title 4"",""Description"": ""Description 4"" }
                }
            },
            ""tp2"": {
                ""Sections"": {
                    ""section1"": {""Title"": ""Title 5"",""Description"": ""Description 5"" },
                    ""section2"": {""Title"": ""Title 6"",""Description"": ""Description 6"" },
                    ""section3"": {""Title"": ""Title 7"",""Description"": ""Description 7"" },
                    ""section4"": {""Title"": ""Title 8"",""Description"": ""Description 8"" }
                }
            },
            ""hr"": {
                ""Sections"": {
                    ""section1"": {""Title"": ""Title 9"",""Description"": ""Description 9"" },
                    ""section2"": {""Title"": ""Title 10"",""Description"": ""Description 10"" },
                    ""section3"": {""Title"": ""Title 11"",""Description"": ""Description 11"" },
                    ""section4"": {""Title"": ""Title 12"",""Description"": ""Description 12"" }
                }
            }
        }
    }";


        [Test]
        public void TestCanConvertJsonToJob()
        {
            Job convertedJob = JsonSerializer.Deserialize<Job>(json);
            Assert.AreEqual(job.ToString(), convertedJob.ToString());
        }
    }
}

