#nullable enable
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xingyi.common.validator
{
    public class ErrorsAnd<T>
    {
        public IEnumerable<string> Errors { get; }
        public T? Value { get; }

        internal ErrorsAnd(IEnumerable<string> errors, T? value)
        {
            Errors = errors;
            Value = value;
        }

        public static ErrorsAnd<T> errors(IEnumerable<string> errors)
        {
            return new ErrorsAnd<T>(errors, default(T?));
        }
        public static ErrorsAnd<T> value(T t)
        {
            return new ErrorsAnd<T>(Enumerable.Empty<string>(), t);
        }
        public static ErrorsAnd<T> errorsOr(IEnumerable<string> errors, T t)
        {
            if (errors.Any())return ErrorsAnd<T>.errors(errors);
            return ErrorsAnd<T>.value(t);
        }

        public Boolean HasErrors()
        {
            return Errors.Any();
        }

        public ErrorsAnd<T1> map<T1>(Func<T, T1> fn)
        {
            if (Errors.Any())
                return ErrorsAnd<T1>.errors(Errors);
            return ErrorsAnd<T1>.value(fn(Value));
        }   
        public ErrorsAnd<T1> flatMap<T1>(Func<T, ErrorsAnd<T1>> fn)
        {
            if (Errors.Any())
                return ErrorsAnd<T1>.errors(Errors);
            return fn(Value);
        }
        public async Task<ErrorsAnd<T1>> mapK<T1>(Func<T, Task<T1>> fn)
        {
            if (Errors.Any())
                return ErrorsAnd<T1>.errors(Errors);
            return ErrorsAnd<T1>.value(await fn(Value));
        }
        public async Task<ErrorsAnd<T1>> flatMapK<T1>(Func<T, Task<ErrorsAnd<T1>>> fn)
        {
            if (Errors.Any())
                return ErrorsAnd<T1>.errors(Errors);
            return await fn(Value);
        }

        public  async Task forHttpResponse( HttpResponse Response, int HappyStatusCode = 200, int FailStatusCode = 400)
        {

            if (this.HasErrors())
            {
                Response.StatusCode = FailStatusCode;
                await Response.WriteAsync(string.Join("\n",Errors));
            }
            else
                Response.StatusCode = HappyStatusCode;
        }
        public  async Task forHttpResponseString( HttpResponse Response, Func<T,string> result, int HappyStatusCode = 200, int FailStatusCode = 400)
        {
             await forHttpResponse(Response, HappyStatusCode, FailStatusCode);
            if (!this.HasErrors())
                await Response.WriteAsync(result(Value));
        }

    }

}
