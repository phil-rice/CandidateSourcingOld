using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xingyi.common
{
    public interface IIdGenerator
    {
        Task<string> GenerateId(string nameSpace);

    }
    public class DefaultIdGenerator : IIdGenerator
    {
        async public Task<string> GenerateId(string nameSpace)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
