using System.Xml;
using VibraScan.Domain.Exceptions;
using VibraScan.Domain.ValueObjects;
using VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base;

namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors
{
    internal class AxisTypeMappingProcessor : ProxyProcessor<AxisType>
    {
        public override string EntityName => "AxisType";

        protected override bool TryMap(IReadOnlyDictionary<string, string> data, out string? rawId, out AxisType? entity, ImportContext context)
        {
            entity = null;

            rawId = data.GetValueOrDefault("C0");
            var name = data.GetValueOrDefault("C1");

            if (!string.IsNullOrWhiteSpace(rawId) && !string.IsNullOrWhiteSpace(name))
            {
                try
                {
                    entity = AxisType.From(name);
                }
                catch (UnsupportedValueException ex)
                {
                    throw new XmlException(ex.Message);
                }

                return true;
            }

            return false;
        }
    }
}