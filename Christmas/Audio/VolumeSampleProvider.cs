using NAudio.Wave;

namespace Christmas.Audio;

class VolumeSampleProvider(ISampleProvider sourceProvider, float volume = 1, bool bypass = false) : ISampleProvider
{
    private readonly ISampleProvider sourceProvider = sourceProvider;
    public bool Bypass { get; set; } = bypass;
    public float Volume { get; set; } = volume;
    public WaveFormat WaveFormat => sourceProvider.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = sourceProvider.Read(buffer, offset, count);

        if (!Bypass)
        {
            for (int i = 0; i < samplesRead; i++)
                buffer[offset + i] = buffer[offset + i] * Volume;
        }

        return samplesRead;
    }
}