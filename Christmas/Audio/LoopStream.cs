using NAudio.Dsp;
using NAudio.Wave;

namespace Christmas.Audio;
public class LoopStream : WaveStream
{
    WaveStream sourceStream;

    public LoopStream(string fileName) : this(new AudioFileReader(fileName))
    {

    }

    /// Creates a new Loop stream
    public LoopStream(WaveStream sourceStream)
    {
        this.sourceStream = sourceStream;
        EnableLooping = true;
    }

    /// Use this to turn looping on or off
    public bool EnableLooping { get; set; }

    /// Return source stream's wave format
    public override WaveFormat WaveFormat
    {
        get { return sourceStream.WaveFormat; }
    }

    /// LoopStream simply returns
    public override long Length
    {
        get { return sourceStream.Length; }
    }

    /// LoopStream simply passes on positioning to source stream
    public override long Position
    {
        get { return sourceStream.Position; }
        set { sourceStream.Position = value; }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;

        while (totalBytesRead < count)
        {
            int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
            if (bytesRead == 0)
            {
                if (sourceStream.Position == 0 || !EnableLooping)
                {
                    // something wrong with the source stream
                    break;
                }
                // loop
                sourceStream.Position = 0;
            }
            totalBytesRead += bytesRead;
        }
        return totalBytesRead;
    }

    protected override void Dispose(bool disposing)
    {
        this.sourceStream.Dispose();
        base.Dispose(disposing);
    }
}

class FilterStream : ISampleProvider
{
    public ISampleProvider SourceProvider { get; }
    private BiQuadFilter filter;

    public FilterStream(ISampleProvider sourceProvider, int cutOffFreq)
    {
        SourceProvider = sourceProvider;
        filter = BiQuadFilter.LowPassFilter(44100, cutOffFreq, 2);
    }

    public WaveFormat WaveFormat { get => SourceProvider.WaveFormat; }

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = SourceProvider.Read(buffer, offset, count);

        for (int i = 0; i < samplesRead; i++)
            buffer[offset + i] = filter.Transform(buffer[offset + i]);

        return samplesRead;
    }
}