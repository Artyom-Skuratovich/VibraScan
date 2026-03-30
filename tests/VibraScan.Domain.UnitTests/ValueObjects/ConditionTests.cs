using FluentAssertions;
using VibraScan.Domain.Exceptions;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Domain.UnitTests.ValueObjects
{
    public class ConditionTests
    {
        [TestCase(1, "Отлично")]
        [TestCase(2, "Удовлетворительно")]
        [TestCase(3, "Не удовлетворительно")]
        [TestCase(4, "Плохо")]
        [TestCase(5, "Не удалось проверить")]
        public void ShouldReturnCorrectDescription(int value, string expectedDescription)
        {
            // Arrange, Act
            var condition = Condition.From(value);

            // Assert
            condition.Description.Should().Be(expectedDescription);
        }

        [Test]
        public void ShouldThrowUnsupportedValueExceptionWhenValueIsInvalid()
        {
            // Act
            var act = () => Condition.From(999);

            // Assert
            act.Should().Throw<UnsupportedValueException>();
        }
    }
}