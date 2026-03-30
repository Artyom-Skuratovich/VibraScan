using FluentAssertions;
using VibraScan.Domain.Entities;
using VibraScan.Domain.Exceptions;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Domain.UnitTests.Entities
{
    public class EngineTests
    {
        [Test]
        public void ShouldUpdatePropertiesAndCalculateNextDateWhenIntervalExists()
        {
            // Arrange
            var engine = new Engine();
            var rule = new InspectionRule { Name = "test" };
            var condition = Condition.Excellent;
            var inspectionDate = DateTime.Now;
            int daysCount = 90;

            rule.AddInterval(condition, daysCount);

            // Act
            engine.ApplyInspection(inspectionDate, condition, rule);

            // Assert
            engine.LastInspectionDate.Should().Be(inspectionDate);
            engine.Condition.Should().Be(condition);
            engine.InspectionRuleId.Should().Be(rule.Id);
            engine.NextInspectionDate.Should().Be(inspectionDate.AddDays(daysCount));
        }

        [Test]
        public void ShouldThrowInspectionIntervalNotFoundExceptionWhenIntervalIsMissing()
        {
            // Arrange
            var engine = new Engine();
            var rule = new InspectionRule { Name = "test" };
            var condition = Condition.Excellent;
            var inspectionDate = DateTime.Now;

            // Act
            var act = () => engine.ApplyInspection(inspectionDate, condition, rule);

            // Assert
            act.Should().Throw<InspectionIntervalNotFoundException>()
                .WithMessage($"В правиле '{rule.Name}' отсутствует настройка интервала для состояния '{condition.Description}'");
        }

        [Test]
        public void ShouldCorrectlyHandleDifferentConditionsInSameRule()
        {
            // Arrange
            var engine = new Engine();
            var rule = new InspectionRule { Name = "test" };
            rule.AddInterval(Condition.Bad, 1);
            rule.AddInterval(Condition.Unsatisfactory, 14);

            var inspectionDate = DateTime.Now;

            // Act
            engine.ApplyInspection(inspectionDate, Condition.Bad, rule);

            // Assert
            engine.NextInspectionDate.Should().Be(inspectionDate.AddDays(1));
        }
    }
}