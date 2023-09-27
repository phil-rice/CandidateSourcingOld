#nullable enable
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xingyi.common.validator
{

    public static class ErrorsAndMaker
    {
        public static ErrorsAnd<T> errors<T>(IEnumerable<string> errors)
        {
            return new ErrorsAnd<T>(errors, default(T?));
        }
        public static ErrorsAnd<T> error<T>(string error)
        {
            return new ErrorsAnd<T>(new List<string> { error}, default(T?));
        }
        public static ErrorsAnd<T> value<T>(T t)
        {
            return new ErrorsAnd<T>(Enumerable.Empty<string>(), t);
        }
        public static ErrorsAnd<T> errorsOr<T>(IEnumerable<string> errors, T t)
        {
            if (errors.Any()) return ErrorsAndMaker.errors<T>(errors);
            return ErrorsAndMaker.value(t);
        }


    }
    public class ErrorsAnd<T>
    {
        public IEnumerable<string> Errors { get; }
        public T? Value { get; }

        internal ErrorsAnd(IEnumerable<string> errors, T? value)
        {
            Errors = errors;
            Value = value;
        }

        public Boolean HasErrors()
        {
            return Errors.Any();
        }

        public ErrorsAnd<T1> map<T1>(Func<T, T1> fn)
        {
            if (Errors.Any())
                return ErrorsAndMaker.errors<T1>(Errors);
            return ErrorsAndMaker.value<T1>(fn(Value));
        }   
        public ErrorsAnd<T1> flatMap<T1>(Func<T, ErrorsAnd<T1>> fn)
        {
            if (Errors.Any())
                return ErrorsAndMaker.errors<T1>(Errors);
            return fn(Value);
        }
        public async Task<ErrorsAnd<T1>> mapK<T1>(Func<T, Task<T1>> fn)
        {
            if (Errors.Any())
                return ErrorsAndMaker.errors<T1>(Errors);
            return ErrorsAndMaker.value<T1>(await fn(Value));
        }
        public async Task<ErrorsAnd<T1>> flatMapK<T1>(Func<T, Task<ErrorsAnd<T1>>> fn)
        {
            if (Errors.Any())
                return ErrorsAndMaker.errors<T1>(Errors);
            return await fn(Value);
        }

        static Exception defaultHandler(IEnumerable<string> errors)
        {
            return new Exception(String.Join(",", errors));
        }
        public T valueOrError(Func<IEnumerable<string>, Exception>? handler = null) 
        {
            var realHandler = handler ?? (Func<IEnumerable<string>, Exception>) defaultHandler;
            if (HasErrors()) throw realHandler(Errors);
            return Value;
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
