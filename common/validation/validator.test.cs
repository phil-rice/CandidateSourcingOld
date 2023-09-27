using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using NUnit.Framework;

namespace xingyi.common.validator
{


    public class ValidatorTests
    {
        [Test]
        public void PredicateValidator_ReturnsErrorMessage_WhenPredicateFails()
        {
            var validator = IValidator<int>.FromPredicate(x => x > 0, x => "Number must be positive");

            var errors = validator.Validate(-1).ToList();

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Number must be positive", errors[0]);
        }

        [Test]
        public void PredicateValidator_NoErrorMessage_WhenPredicatePasses()
        {
            var validator = IValidator<int>.FromPredicate(x => x > 0, x => "Number must be positive");

            var errors = validator.Validate(5).ToList();

            Assert.IsEmpty(errors);
        }

        [Test]
        public void CompositeValidator_ReturnsAllErrorMessages()
        {
            var validator = IValidator<int>.Compose(
                IValidator<int>.FromPredicate(x => x > 0, x => "Number must be positive"),
                IValidator<int>.FromPredicate(x => x % 2 == 0, x => "Number must be even")
            );

            var errors = validator.Validate(-5).ToList();

            Assert.AreEqual(2, errors.Count);
            Assert.Contains("Number must be positive", errors);
            Assert.Contains("Number must be even", errors);
        }

        [Test]
        public void CompositeValidator_NoErrorMessages_WhenAllValid()
        {
            var validator = IValidator<int>.Compose(
                IValidator<int>.FromPredicate(x => x > 0, x => "Number must be positive"),
              IValidator<int>.FromPredicate(x => x % 2 == 0, x => "Number must be even")
            );

            var errors = validator.Validate(6).ToList();

            Assert.IsEmpty(errors);
        }
    }
}

