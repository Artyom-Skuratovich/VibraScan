using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Domain.Entities;

namespace VibraScan.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
    {
        private IDbContextTransaction? _currentTransaction;

        public DbSet<Engine> Engines => Set<Engine>();

        public DbSet<InspectionInterval> InspectionIntervals => Set<InspectionInterval>();

        public DbSet<InspectionRule> InspectionRules => Set<InspectionRule>();

        public DbSet<MeasurementProfile> MeasurementProfiles => Set<MeasurementProfile>();

        public DbSet<Point> Points => Set<Point>();

        public DbSet<VibrationMeasurement> VibrationMeasurements => Set<VibrationMeasurement>();

        public DbSet<Workshop> Workshops => Set<Workshop>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _currentTransaction ??= await Database.BeginTransactionAsync(ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(ct);
                }
            }
            catch
            {
                await RollbackTransactionAsync(ct);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(ct);
                }
            }
            catch
            {
                // Игнорируем (например, при разрыве соединения с БД).
                // Сервер БД закроет транзакцию автоматически.
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}