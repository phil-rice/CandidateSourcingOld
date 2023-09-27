using xingyi.common.validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace common.functions
{

    static class NonFunctionals
    {

        static Func<From, To> atStartDo<From, To>(Func<From, To> func, Action<From> action)
        {
            return from =>
            {
                action(from);
                return func(from);
            };
        }
        static Func<From, To> atEndDo<From, To>(Func<From, To> func, Action<To> action)
        {
            return from =>
            {
                var to = func(from);
                action(to);
                return to;
            };
        }
  

    }
}
