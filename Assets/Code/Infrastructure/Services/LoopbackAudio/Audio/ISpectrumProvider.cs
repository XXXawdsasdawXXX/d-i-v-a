namespace Code.Infrastructure.Services.LoopbackAudio.Audio
{
    internal interface ISpectrumProvider
    {
        bool GetFftData(float[] fftBuffer, object context);
        int GetFftBandIndex(float frequency);
    }
}