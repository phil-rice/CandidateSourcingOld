using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Xunit;

namespace xingyi.common.validator
{


    public class ValidatorTests
    {
        [Fact]
        public void PredicateValidator_ReturnsErrorMessage_WhenPredicateFails()
        {
            var validator = IValidator<int>.FromPredicate(x => x > 0, x => "Number must be positive");

            var errors = validator.Validate(-1).ToList();

            Assert.Single(errors);
            Assert.Equal("Number must be positive", errors[0]);
        }

        [Fact]
        public void PredicateValidator_NoErrorMessage_WhenPredicatePasses()
        {
            var validator = IValidator<int>.FromPredicate(x => x > 0, x => "Number must be positive");

            var errors = validator.Validate(5).ToList();

            Assert.Empty(errors);
        }

        [Fact]
        public void CompositeValidator_ReturnsAllErrorMessages()
        {
            var validator = IValidator<int>.Compose(
                IValidator<int>.FromPredicate(x => x > 0, x => "Number must be positive"),
                IValidator<int>.FromPredicate(x => x % 2 == 0, x => "Number must be even")
            );

            var errors = validator.Validate(-5).ToList();

            Assert.Equal(2, errors.Count);
            Assert.Contains("Number must be positive", errors);
            Assert.Contains("Number must be even", errors);
        }

        [Fact]
        public void CompositeValidator_NoErrorMessages_WhenAllValid()
        {
            var validator = IValidator<int>.Compose(
                IValidator<int>.FromPredicate(x => x > 0, x => "Number must be positive"),
              IValidator<int>.FromPredicate(x => x % 2 == 0, x => "Number must be even")
            );

            var errors = validator.Validate(6).ToList();

            Assert.Empty(errors);
        }
    }
}

