using VibraScan.Domain.Entities;
using VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base;
using VibraScan.Infrastructure.DataImport.StartImport.Models;

namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors
{
    internal class ProxyScheduleProcessor : ProxyProcessor<ProxySchedule>
    {
        public override string EntityName => "Schedule";

        protected override bool TryMap(IReadOnlyDictionary<string, string> data, out string? rawId, out ProxySchedule? entity, ImportContext context)
        {
            entity = null;

            rawId = data.GetValueOrDefault("C0");
            var axisId = data.GetValueOrDefault("C1");
            var measurementProfileId = data.GetValueOrDefault("C4");

            if (!string.IsNullOrWhiteSpace(rawId) && !string.IsNullOrWhiteSpace(axisId) && !string.IsNullOrWhiteSpace(measurementProfileId))
            {
                var proxyAxis = context.Get<ProxyAxis>(axisId)
                                ?? throw new ImportInconsistencyException($"Сущность Axis с Id='{axisId}' не найдена для сущности Schedule с Id='{rawId}'");

                var measurementProfile = context.Get<MeasurementProfile>(measurementProfileId)
                                         ?? throw new ImportInconsistencyException($"Сущность ParamSet с Id='{measurementProfileId}' не найдена для сущности Schedule с Id='{rawId}'");

                entity = new ProxySchedule
                {
                    AxisType = proxyAxis.AxisType,
                    PointId = proxyAxis.PointId,
                    MeasurementProfileId = measurementProfile.Id
                };

                return true;
            }

            return false;
        }
    }
}