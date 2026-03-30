using System.Globalization;
using System.Xml;
using VibraScan.Domain.Entities;
using VibraScan.Domain.Exceptions;
using VibraScan.Domain.ValueObjects;
using VibraScan.Infrastructure.Data.Bulk;
using VibraScan.Infrastructure.DataImport.StartImport.Actions;
using VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base;
using VibraScan.Infrastructure.DataImport.StartImport.Models;

namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors
{
    internal class VibrationMeasurementProcessor(IBulkOperations bulk, IEnumerable<IAfterSaveAction<VibrationMeasurement>> actions)
        : EntityProcessor<VibrationMeasurement>(bulk, actions)
    {
        public override string EntityName => "Record";

        protected override bool TryMap(IReadOnlyDictionary<string, string> data, out string? rawId, out VibrationMeasurement? entity, ImportContext context)
        {
            entity = null;

            rawId = data.GetValueOrDefault("C0");
            var scheduleId = data.GetValueOrDefault("C1");
            var capturedAt = ParseDateTimeWithMicroseconds(data.GetValueOrDefault("C2"));
            var rms = float.TryParse(data.GetValueOrDefault("C12"), NumberStyles.Any, CultureInfo.InvariantCulture, out var rRms) ? rRms : -1;
            var measurementDomain = int.TryParse(data.GetValueOrDefault("C15"), out var rMeasurementDomain) ? rMeasurementDomain : -1;
            var base64Data = data.GetValueOrDefault("C18");

            if (!string.IsNullOrWhiteSpace(scheduleId) && capturedAt.HasValue && (rms >= 0) && (measurementDomain >= 0) && !string.IsNullOrWhiteSpace(base64Data))
            {
                var proxySchedule = context.Get<ProxySchedule>(scheduleId)
                                    ?? throw new ImportInconsistencyException($"Сущность Schedule с Id='{scheduleId}' не найдена для сущности Record с Id='{rawId}'");

                try
                {
                    entity = new VibrationMeasurement
                    {
                        AxisType = proxySchedule.AxisType,
                        PointId = proxySchedule.PointId,
                        MeasurementProfileId = proxySchedule.MeasurementProfileId,
                        CapturedAt = capturedAt.Value,
                        Rms = rms,
                        MeasurementDomain = MeasurementDomain.From(measurementDomain),
                        RawData = Convert.FromBase64String(base64Data)
                    };
                }
                catch (FormatException)
                {
                    throw new XmlException("Не удалось преобразовать строку на позиции <C18> из Base64 в массив байтов. Неверный формат строки");
                }
                catch (UnsupportedValueException ex)
                {
                    throw new XmlException(ex.Message);
                }

                return true;
            }

            return false;
        }

        private static DateTime? ParseDateTimeWithMicroseconds(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return DateTime.TryParseExact(
                value,
                "yyyy/MM/dd HH:mm:ss-ffffff",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result) ? result : null;
        }
    }
}