
using xingyi.cas.common;

namespace xingyi.cas.client
{
    [Serializable]
    internal class CasNotJsonException : Exception
    {
        private ContentItem contentItem;

        public CasNotJsonException(cas.common.ContentItem contentItem)
        {
            this.contentItem = contentItem;
        }
        public override string Message => $"Not Json: {contentItem}";
    }

    [Serializable]
    internal class CasNotFoundException : Exception
    {
        private string nameSpace;
        private string sha;


        public CasNotFoundException(string nameSpace, string sha)
        {
            this.nameSpace = nameSpace;
            this.sha = sha;
        }

        public override string Message => $"Cannot find item in Cas with {nameSpace}/${sha}";
    }
}
