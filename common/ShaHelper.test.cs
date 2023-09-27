namespace xingyi.common
{
    using System.Text;
    using NUnit.Framework;

    public class SHAHelperTests
    {
        IShaCodec codex = new ShaCodec();

        [Test]
        public void TestComputeSha()
        {
            // Sample content and its corresponding SHA
            byte[] content = Encoding.UTF8.GetBytes("SampleContent");
            string expectedSha = "sKpT6KOKK6kwIvgbWwjyiTQuzn3mH4OUM55oDoaepl4";

            string computedSha = codex.ComputeSha(content);

            Assert.AreEqual(expectedSha, computedSha);
        }

        [Test]
        public void TestValidateCorrectSha()
        {
            byte[] content = Encoding.UTF8.GetBytes("SampleContent");
            string correctSha = codex.ComputeSha(content);

            bool isValid = codex.Validate(content, correctSha);

            Assert.True(isValid);
        }

        [Test]
        public void TestValidateIncorrectSha()
        {
            byte[] content = Encoding.UTF8.GetBytes("SampleContent");
            string incorrectSha = "INCORRECT_SHA";

            bool isValid = codex.Validate(content, incorrectSha);

            Assert.False(isValid);
        }
    }

}
