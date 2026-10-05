using MediatR;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public record GetChartsQuery(
        DateTime CapturedAt,
        long PointId,
        long MeasurementProfileId,
        AxisType AxisType) : IRequest<ChartsResponse?>, ICachableQuery
    {
        public string ChacheKey => $"chart_{CapturedAt:yyyyMMdd}_{PointId}_{MeasurementProfileId}_{AxisType}";

        public TimeSpan ExpirationTime => TimeSpan.FromMinutes(5);
    }
}