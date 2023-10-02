using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace xingyi.common
{
    public static class Json
    {
        public static T transform<T>(Dictionary<string, object> data)
        {
            var json = JsonSerializer.Serialize(data);
            return JsonSerializer.Deserialize<T>(json);
        }
        public static List<T> transformList<T>(List<Dictionary<string, object>> data)
        {
            return data.Select(dict => transform<T>(dict)).ToList();
        }

        public static byte[] getBytes<T>(T t)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(t)); 
        }
    }
}
