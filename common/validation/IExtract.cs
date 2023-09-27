using xingyi.common.validator;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xingyi.common.validator
{
    public interface IExtract<From, To>
    {
        ErrorsAnd<To> extract(From from);


    }

    public class Extract
    {

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public static IExtract<From, To> CreateExtractor<From, To>(Func<From, To> func, IValidator<To> validator)
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
        {
            return new LambdaExtract<From, To>(from =>
            {
                var t = func(from);
                var errors = validator.Validate(t);
                if (errors.Any()) return ErrorsAndMaker.errors<To>(errors);
                return ErrorsAndMaker.value<To>(func(from));
            });
        }

        public static IExtract<HttpRequest, string> fromHeader(string headerName)
        {
            var validator = IValidator<string>.notEmpty("Header " + headerName + " not in request");
            return CreateExtractor<HttpRequest, string>(req => req.Headers[headerName].FirstOrDefault(), validator);


        }

        public static IExtract<From, To> Compose<From, T1, T2, To>(
   IExtract<From, T1> e1,
   IExtract<From, T2> e2,
   Func<T1, T2, To> merge)
        {
            return new LambdaExtract<From, To>(from =>
            {
                var result1 = e1.extract(from);
                var result2 = e2.extract(from);
                if (result1.HasErrors() || result2.HasErrors())

                    return ErrorsAndMaker.errors<To>(result1.Errors.Concat(result2.Errors));
                else
                    return ErrorsAndMaker.value<To>(merge(result1.Value, result2.Value));
            });
        }
        public static IExtract<From, To> Compose<From, T1, T2, T3, To>(
 IExtract<From, T1> e1,
 IExtract<From, T2> e2,
 IExtract<From, T3> e3,
 Func<T1, T2, T3, To> merge)
        {
            return new LambdaExtract<From, To>(from =>
            {
                var result1 = e1.extract(from);
                var result2 = e2.extract(from);
                var result3 = e3.extract(from);
                if (result1.HasErrors() || result2.HasErrors() || result3.HasErrors())

                    return ErrorsAndMaker.errors<To>(result1.Errors.Concat(result2.Errors).Concat(result3.Errors));
                else
                    return ErrorsAndMaker.value<To>(merge(result1.Value, result2.Value, result3.Value));
            });
        }
        public static IExtract<From, To> Compose<From, T1, T2, T3, T4, To>(
 IExtract<From, T1> e1,
 IExtract<From, T2> e2,
 IExtract<From, T3> e3,
 IExtract<From, T4> e4,
 Func<T1, T2, T3, T4, To> merge)
        {
            return new LambdaExtract<From, To>(from =>
            {
                var result1 = e1.extract(from);
                var result2 = e2.extract(from);
                var result3 = e3.extract(from);
                var result4 = e4.extract(from);
                if (result1.HasErrors() || result2.HasErrors() || result3.HasErrors() || result4.HasErrors())

                    return ErrorsAndMaker.errors<To>(result1.Errors.Concat(result2.Errors).Concat(result3.Errors).Concat(result4.Errors));
                else
                    return ErrorsAndMaker.value<To>(merge(result1.Value, result2.Value, result3.Value, result4.Value));
            });
        }

    }


    public class LambdaExtract<From, To> : IExtract<From, To>
    {
        private readonly Func<From, ErrorsAnd<To>> _extractor;

        public LambdaExtract(Func<From, ErrorsAnd<To>> extractor)
        {
            _extractor = extractor;
        }

        public ErrorsAnd<To> extract(From from) => _extractor(from);
    }

}
