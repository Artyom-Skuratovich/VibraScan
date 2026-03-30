using VibraScan.Domain.ValueObjects;

namespace VibraScan.Domain.Exceptions
{
    public class InspectionIntervalNotFoundException(string ruleName, Condition condition)
        : Exception($"В правиле '{ruleName}' отсутствует настройка интервала для состояния '{condition.Description}'")
    {
    }
}