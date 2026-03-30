using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;
using VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base;
using VibraScan.Infrastructure.DataImport.StartImport.Models;

namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors
{
    internal class ProxyAxisProcessor : ProxyProcessor<ProxyAxis>
    {
        public override string EntityName => "Axis";

        protected override bool TryMap(IReadOnlyDictionary<string, string> data, out string? rawId, out ProxyAxis? entity, ImportContext context)
        {
            entity = null;

            rawId = data.GetValueOrDefault("C0");
            var pointId = data.GetValueOrDefault("C1");
            var axisTypeId = data.GetValueOrDefault("C2");

            if (!string.IsNullOrWhiteSpace(rawId) && !string.IsNullOrWhiteSpace(pointId) && !string.IsNullOrWhiteSpace(axisTypeId))
            {
                var axisType = context.Get<AxisType>(axisTypeId)
                               ?? throw new ImportInconsistencyException($"Сущность AxisType с Id='{axisTypeId}' не найдена для сущности Axis с Id='{rawId}'");

                var point = context.Get<Point>(pointId)
                            ?? throw new ImportInconsistencyException($"Сущность Point с Id='{pointId}' не найдена для сущности Axis с Id='{rawId}'");

                entity = new ProxyAxis
                {
                    AxisType = axisType,
                    PointId = point.Id
                };

                return true;
            }

            return false;
        }
    }
}