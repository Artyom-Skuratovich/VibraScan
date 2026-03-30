using VibraScan.Domain.ValueObjects;

namespace VibraScan.Domain.Exceptions
{
    public class DuplicateInspectionIntervalException(Condition condition, string ruleName)
        : Exception($"Для правила '{ruleName}' уже задан интервал проверки с состоянием '{condition.Description}'")
    {
    }
}