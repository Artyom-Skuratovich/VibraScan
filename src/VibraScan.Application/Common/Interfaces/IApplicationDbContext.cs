using Microsoft.EntityFrameworkCore;
using VibraScan.Domain.Entities;

namespace VibraScan.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Engine> Engines { get; }

        DbSet<InspectionInterval> InspectionIntervals { get; }

        DbSet<InspectionRule> InspectionRules { get; }

        DbSet<MeasurementProfile> MeasurementProfiles { get; }

        DbSet<Point> Points { get; }

        DbSet<VibrationMeasurement> VibrationMeasurements { get; }

        DbSet<Workshop> Workshops { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task BeginTransactionAsync(CancellationToken ct = default);

        Task CommitTransactionAsync(CancellationToken ct = default);

        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}