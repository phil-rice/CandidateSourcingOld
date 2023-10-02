using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xingyi.common
{
    using System.Security.Cryptography;
    using System.Text;

    public static class Guids
    {
        public static Guid from(string input)
        {
            // Use MD5 to get a 16-byte hash of the string
            using (var provider = MD5.Create())
            {
                byte[] inputBytes = Encoding.Default.GetBytes(input);
                byte[] hashBytes = provider.ComputeHash(inputBytes);
                return new Guid(hashBytes);
            }
        }
    }

}
