using System.Xml;
using VibraScan.Application.DataImport;

namespace VibraScan.Infrastructure.DataImport.StartImport
{
    public interface IEntityProcessor
    {
        string EntityName { get; }

        Task ProcessAsync(XmlReader reader, ImportContext context, CancellationToken ct = default);

        Task CommitAsync(ImportContext context, CancellationToken ct = default);
    }
}