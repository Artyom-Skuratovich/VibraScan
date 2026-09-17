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
        private const float MillimeterScaleFactor = 1000f;
        private readonly IApplicationDbContext _context = context;

        private record HistoryItem(MeasurementDomain MeasurementDomain, DateTime CapturedAt, float Rms);

        public async Task<ChartsResponse?> Handle(GetChartsQuery request, CancellationToken ct)
        {
            var measurements = await GetCurrentMeasurementsAsync(request, ct);

            if (measurements.Count == 0)
            {
                return null;
            }

            var rmsHistory = await GetRmsHistoryAsync(request, ct);

            var freqBundle = CreateBundle<FrequencyDomainChart>(measurements, rmsHistory, MeasurementDomain.Frequency);

            var scaleFactor = freqBundle is null ? MillimeterScaleFactor : 1f;

            var timeBundle = CreateBundle<TimeDomainChart>(measurements, rmsHistory, MeasurementDomain.Time, scaleFactor);

            return new ChartsResponse(timeBundle, freqBundle);
        }

        private async Task<List<VibrationMeasurement>> GetCurrentMeasurementsAsync(GetChartsQuery request, CancellationToken ct)
        {
            return await _context.VibrationMeasurements
                .AsNoTracking()
                .Where(m => m.PointId == request.PointId
                            && m.MeasurementProfileId == request.MeasurementProfileId
                            && m.AxisType == request.AxisType
                            && m.CapturedAt == request.CapturedAt)
                .ToListAsync(ct);
        }

        private async Task<List<HistoryItem>> GetRmsHistoryAsync(GetChartsQuery request, CancellationToken ct)
        {
            return await _context.VibrationMeasurements
                .AsNoTracking()
                .Where(m => m.PointId == request.PointId
                            && m.MeasurementProfileId == request.MeasurementProfileId
                            && m.AxisType == request.AxisType)
                .OrderBy(m => m.CapturedAt)
                .Select(m => new HistoryItem(m.MeasurementDomain, m.CapturedAt, m.Rms))
                .ToListAsync(ct);
        }

        private static ChartBundle<T>? CreateBundle<T>(
            IEnumerable<VibrationMeasurement> measurements,
            IEnumerable<HistoryItem> historyItems,
            MeasurementDomain domain,
            float scaleFactor = 1f)
            where T : SingleMeasurementChart
        {
            var measurement = measurements.FirstOrDefault(m => m.MeasurementDomain == domain);

            if (measurement == null)
            {
                return null;
            }

            var measurementChart = MapToSingleMeasurementChart(measurement, scaleFactor) as T;
            var rmsHistoryChart = MapToHistoryChart(historyItems, domain, scaleFactor);

            return new ChartBundle<T>(measurementChart!, rmsHistoryChart!);
        }

        private static SingleMeasurementChart? MapToSingleMeasurementChart(VibrationMeasurement measurement, float scaleFactor = 1f)
        {
            if ((measurement.RawData == null) || (measurement.RawData.Length == 0))
            {
                return null;
            }

            var values = MemoryMarshal.Cast<byte, float>(measurement.RawData).ToArray();

            for (int i = 0; i < values.Length && (scaleFactor != 1f); i++)
            {
                values[i] *= scaleFactor;
            }

            if (measurement.MeasurementDomain == MeasurementDomain.Frequency)
            {
                var range = CalculateAmplitudeRange(values);
                return new FrequencyDomainChart(measurement.Rms * scaleFactor, values, range);
            }

            return new TimeDomainChart(measurement.Rms * scaleFactor, values);
        }

        private static RmsHistoryChart? MapToHistoryChart(IEnumerable<HistoryItem> historyItems, MeasurementDomain domain, float scaleFactor = 1f)
        {
            var points = historyItems
                .Where(h => h.MeasurementDomain == domain)
                .Select(h => new RmsHistoryPoint(h.CapturedAt, h.Rms * scaleFactor))
                .ToList();

            return points.Count > 0 ? new RmsHistoryChart(points) : null;
        }

        private static float CalculateAmplitudeRange(float[] values)
        {
            if (values.Length == 0)
            {
                return 0;
            }

            float min = values[0], max = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                var current = values[i];

                if (current < min)
                {
                    min = current;
                }
                else if (current > max)
                {
                    max = current;
                }
            }

            return max - min;
        }
    }
}