using FluentAssertions;
using VibraScan.Domain.Entities;
using VibraScan.Domain.Exceptions;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Domain.UnitTests.Entities
{
    public class InspectionRuleTests
    {
        [Test]
        public void ShouldAddIntervalWhenConditionIsUnique()
        {
            // Arrange
            var rule = new InspectionRule { Name = "test" };
            var condition = Condition.Satisfactory;

            // Act
            rule.AddInterval(condition, 365);

            // Assert
            rule.Intervals.Should().HaveCount(1);
            rule.Intervals.First().TargetCondition.Should().Be(condition);
        }

        [Test]
        public void ShouldThrowDuplicateInspectionIntervalExceptionWhenConditionAlreadyExists()
        {
            // Arrange
            var rule = new InspectionRule { Name = "test" };
            var condition = Condition.Satisfactory;
            rule.AddInterval(condition, 180);

            // Act
            var act = () => rule.AddInterval(condition, 90);

            // Assert
            act.Should().Throw<DuplicateInspectionIntervalException>().WithMessage("*уже задан интервал*");
        }

        [Test]
        public void IntervalsShouldBeReadOnly()
        {
            // Arrange
            var rule = new InspectionRule { Name = "test" };

            // Assert
            rule.Intervals.Should().BeAssignableTo<IReadOnlyCollection<InspectionInterval>>();
            rule.Intervals.Should().NotBeAssignableTo<List<InspectionInterval>>();
        }
    }
}