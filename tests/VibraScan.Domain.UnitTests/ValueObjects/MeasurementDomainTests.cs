using FluentAssertions;
using VibraScan.Domain.Exceptions;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Domain.UnitTests.ValueObjects
{
    public class MeasurementDomainTests
    {
        [TestCase(1, "Временная область")]
        [TestCase(2, "Частотная область")]
        public void ShouldReturnCorrectName(int value, string expectedName)
        {
            // Arrange, Act
            var measurementDomain = MeasurementDomain.From(value);

            // Assert
            measurementDomain.Name.Should().Be(expectedName);
        }

        [Test]
        public void ShouldThrowUnsupportedValueExceptionWhenValueIsInvalid()
        {
            // Act
            var act = () => MeasurementDomain.From(999);

            // Assert
            act.Should().Throw<UnsupportedValueException>();
        }
    }
}