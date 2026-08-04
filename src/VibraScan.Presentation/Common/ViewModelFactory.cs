namespace VibraScan.Presentation.Common
{
    public delegate TVm ViewModelFactory<in TParam, out TVm>(TParam param);
}