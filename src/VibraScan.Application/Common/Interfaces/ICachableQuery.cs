namespace VibraScan.Application.Common.Interfaces
{
    public interface ICachableQuery
    {
        string ChacheKey { get; }

        TimeSpan ExpirationTime { get; }
    }
}