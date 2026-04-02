namespace VibraScan.Application.DataImport
{
    public readonly struct ImportSource(string name, Stream data)
    {
        public string Name { get; } = name;

        public Stream Data { get; } = data;
    }
}