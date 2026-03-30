using MediatR;

namespace VibraScan.Application.DataImport.Commands.StartImport
{
    public record StartImportCommand : IRequest<ImportResult>
    {
        public Stream DataStream { get; init; } = null!;

        public IProgress<ImportProgress> Progress { get; init; } = null!;
    }
}