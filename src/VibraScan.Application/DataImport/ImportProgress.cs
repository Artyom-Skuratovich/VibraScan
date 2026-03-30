namespace VibraScan.Application.DataImport
{
    public readonly struct ImportProgress(double percentage, string currentStage)
    {
        public double Percentage { get; } = percentage;

        public string CurrentStage { get; } = currentStage;
    }
}