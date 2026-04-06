using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public class GetChartsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetChartsQuery, ChartsResponse?>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<ChartsResponse?> Handle(GetChartsQuery request, CancellationToken ct)
        {
            var measurements = await _context.VibrationMeasurements
                .AsNoTracking()
                .Where(m => m.PointId == request.PointId
                            && m.MeasurementProfileId == request.MeasurementProfileId
                            && m.AxisType == request.AxisType
                            && m.CapturedAt == request.CapturedAt)
                .ToListAsync(ct);

            if (measurements.Count == 0) return null;

            TimeDomainChart? timeChart = null;
            FrequencyDomainChart? frequencyChart = null;

            foreach (var m in measurements)
            {
                var chart = MapToChart(m);

                if (chart is FrequencyDomainChart freq) frequencyChart = freq;
                else if (chart is TimeDomainChart time) timeChart = time;
            }

            return (frequencyChart == null && timeChart == null)
                ? null
                : new ChartsResponse(timeChart, frequencyChart);
        }

        private static BaseChart? MapToChart(VibrationMeasurement measurement)
        {
            if ((measurement.RawData == null) || (measurement.RawData.Length == 0)) return null;

            var values = MemoryMarshal.Cast<byte, float>(measurement.RawData).ToArray();

            if (measurement.MeasurementDomain == MeasurementDomain.Frequency)
            {
                float range = 0;

                if (values.Length != 0)
                {
                    float min = values[0], max = values[0];

                    for (int i = 1; i < values.Length; i++)
                    {
                        var current = values[i];

                        if (current < min) min = current;
                        else if (current > max) max = current;
                    }

                    range = max - min;
                }

                return new FrequencyDomainChart(measurement.MeasurementDomain.Name, measurement.Rms, values, range);
            }

            return new TimeDomainChart(measurement.MeasurementDomain.Name, measurement.Rms, values);
        }
    }
}