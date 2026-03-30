namespace VibraScan.Domain.Exceptions
{
    public class UnsupportedValueException(string value, string type)
        : Exception($"Значение '{value}' не поддерживается для типа '{type}'")
    {
    }
}