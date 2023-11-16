using NAudio.Wave;

namespace Christmas.Audio;
class AutoDisposeFileReader(AudioFileReader reader) : ISampleProvider
{
    private readonly AudioFileReader reader = reader;
    private bool isDisposed;

    public int Read(float[] buffer, int offset, int count)
    {
        if (isDisposed)
            return 0;
        int read = reader.Read(buffer, offset, count);
        if (read == 0)
        {
            reader.Dispose();
            isDisposed = true;
        }
        return read;
    }

    public WaveFormat WaveFormat { get; private set; } = reader.WaveFormat;
}