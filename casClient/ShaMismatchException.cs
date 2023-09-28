using System.Runtime.Serialization;

namespace xingyi.cas.client
{
    [Serializable]
    internal class ShaMismatchException : Exception
    {
        private string sha;
        private string checkedSha;
        private byte[] data;

        public ShaMismatchException(string sha, string checkedSha, byte[] data)
        {
            this.sha = sha;
            this.checkedSha = checkedSha;
            this.data = data;
        }

        public override string Message
        {
            get
            {
                return string.Format("Sha mismatch. Requested sha {0} and found {1}", sha, checkedSha);
            }
        }
    }
}