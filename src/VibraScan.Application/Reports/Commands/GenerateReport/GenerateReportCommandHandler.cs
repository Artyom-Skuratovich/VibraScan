using MediatR;
using Microsoft.EntityFrameworkCore;
using VibraScan.Application.Common.Interfaces;

namespace VibraScan.Application.Reports.Commands.GenerateReport
{
    public class GenerateReportCommandHandler(IApplicationDbContext context, IReportStorageService storageService) : IRequestHandler<GenerateReportCommand>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IReportStorageService _storageService = storageService;

        public async Task Handle(GenerateReportCommand request, CancellationToken ct)
        {
            var header = await GetHeaderMetadataAsync(request.EngineId, request.MeasurementProfileId, ct);
            var dataPackage = await LoadAndFilterMeasurementDataAsync(request, ct);
            var reportRows = BuildReportRows(dataPackage.Measurements, dataPackage.ActivePointIds);

            var reportData = new ReportData(
                header!.EngineName,
                header.ProfileName!,
                request.AxisType,
                dataPackage.ColumnNames,
                reportRows);

            await _storageService.SaveReportAsync(reportData, request.DestinationPath, ct);
        }

        private async Task<HeaderMetadata> GetHeaderMetadataAsync(long engineId, long profileId, CancellationToken ct)
        {
            var header = await _context.Engines
                .AsNoTracking()
                .Where(e => e.Id == engineId)
                .Select(e => new HeaderMetadata
                {
                    EngineName = e.Name,
                    ProfileName = _context.MeasurementProfiles
                        .AsNoTracking()
                        .Where(p => p.Id == profileId)
                        .Select(p => p.Description)
                        .FirstOrDefault()!
                })
                .FirstOrDefaultAsync(ct);

            return header!;
        }

        private async Task<MeasurementDataPackage> LoadAndFilterMeasurementDataAsync(GenerateReportCommand request, CancellationToken ct)
        {
            var allPoints = await _context.Points
                .AsNoTracking()
                .Where(p => p.EngineId == request.EngineId)
                .OrderBy(p => p.Id)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync(ct);

            var allPointIds = allPoints.Select(p => p.Id).ToList();

            var measurements = await _context.VibrationMeasurements
                .AsNoTracking()
                .Where(m => allPointIds.Contains(m.PointId)
                            && m.MeasurementProfileId == request.MeasurementProfileId
                            && m.AxisType == request.AxisType
                            && m.CapturedAt >= request.From
                            && m.CapturedAt <= request.To)
                .Select(m => new MeasurementItem(m.CapturedAt.Date, m.PointId, m.Rms))
                .ToListAsync(ct);

            var activePointIdsWithData = measurements
                .Select(m => m.PointId)
                .Distinct()
                .ToHashSet();

            var activePoints = allPoints
                .Where(p => activePointIdsWithData.Contains(p.Id))
                .ToList();

            return new MeasurementDataPackage
            {
                ColumnNames = [.. activePoints.Select(p => p.Name)],
                ActivePointIds = [.. activePoints.Select(p => p.Id)],
                Measurements = measurements
            };
        }

        private static List<ReportRow> BuildReportRows(List<MeasurementItem> measurements, List<long> activePointIds)
        {
            return [.. measurements
                .GroupBy(m => m.Date)
                .OrderBy(g => g.Key)
                .Select(dateGroup => new ReportRow(
                    Date: dateGroup.Key,
                    Values: [.. activePointIds.Select(id =>
                    {
                        var pointMeasures = dateGroup.Where(m => m.PointId == id).ToList();
                        return pointMeasures.Count != 0 ?(float?) pointMeasures.Average(m => m.Rms) : null;
                    })]
                ))];
        }

        private class HeaderMetadata
        {
            public string EngineName { get; set; } = null!;

            public string ProfileName { get; set; } = null!;
        }

        private class MeasurementDataPackage
        {
            public List<string> ColumnNames { get; set; } = [];

            public List<long> ActivePointIds { get; set; } = [];

            public List<MeasurementItem> Measurements { get; set; } = [];
        }

        private record MeasurementItem(DateTime Date, long PointId, float Rms);
    }
}