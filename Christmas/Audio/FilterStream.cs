using NAudio.Dsp;
using NAudio.Wave;

namespace Christmas.Audio;

class FilterStream(ISampleProvider sourceProvider, int cutoff, bool bypass = false) : ISampleProvider
{
    private readonly ISampleProvider sourceProvider = sourceProvider;
    private readonly BiQuadFilter filter = BiQuadFilter.LowPassFilter(44100, cutoff, 2);
    public bool Bypass { get; set; } = bypass;
    public float Cutoff { set => filter.SetLowPassFilter(44100, cutoff, 2); }
    public WaveFormat WaveFormat => sourceProvider.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = sourceProvider.Read(buffer, offset, count);

        if (!Bypass)
        {
            for (int i = 0; i < samplesRead; i++)
                buffer[offset + i] = filter.Transform(buffer[offset + i]);
        }

        return samplesRead;
    }
}