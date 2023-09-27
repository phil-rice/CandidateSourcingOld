
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xingyi.common.validator
{
    public interface IValidator<T>
    {
        IEnumerable<string> Validate(T item);

        public static IValidator<T> FromPredicate(Func<T, bool> predicate, Func<T, string> errorMessage)
        {
            return new PredicateValidator<T>(predicate, errorMessage);
        }
        public static IValidator<T> Compose(params IValidator<T>[] validators)
        {
            return new CompositeValidator<T>(validators);
        }
        public static IValidator<string> notEmpty(string message)
        {
            return IValidator<string>.FromPredicate(s => !String.IsNullOrEmpty(s), ignore=>message);
        }

    }


    public static class ValidatorExtensions
    {

        public static async Task<HttpResponse> writeErrors(HttpResponse Response, int StatusCode, IEnumerable<string> Errors)
        {

            Response.StatusCode = StatusCode;
            if (Errors.Any())
            {
                await Response.WriteAsync(string.Join("\n", Errors));

            }

            return Response;
        }



    }

    public class PredicateValidator<T> : IValidator<T>
    {
        private readonly Func<T, bool> _predicate;
        private readonly Func<T, string> _errorMessage;

        public PredicateValidator(Func<T, bool> predicate, Func<T, string> errorMessage)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _errorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }

        public IEnumerable<string> Validate(T item)
        {
            if (!_predicate(item))
            {
                yield return _errorMessage(item);
            }
        }
    }

    public class CompositeValidator<T> : IValidator<T>
    {
        private readonly IValidator<T>[] _validators;

        public CompositeValidator(params IValidator<T>[] validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public IEnumerable<string> Validate(T item)
        {
            return _validators.SelectMany(validator => validator.Validate(item));
        }
    }


}
