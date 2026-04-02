using MediatR;

namespace VibraScan.Application.DataImport.Commands.StartImport
{
    public record StartImportCommand : IRequest<ImportResult>
    {
        public ImportSource Source { get; init; }

        public IProgress<ImportProgress> Progress { get; init; } = null!;
    }
}