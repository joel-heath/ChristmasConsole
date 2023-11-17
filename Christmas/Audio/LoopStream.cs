using NAudio.Wave;
using NLayer.NAudioSupport;

namespace Christmas.Audio;
public class LoopStream(WaveStream sourceStream) : WaveStream
{
    private readonly WaveStream sourceStream = sourceStream;

    public LoopStream(string fileName) : this(new WaveFileReader(fileName)) { } //: this(new ManagedMpegStream(fileName)) { }

    public bool EnableLooping { get; set; } = true;

    public override WaveFormat WaveFormat => sourceStream.WaveFormat;
    public override long Length => sourceStream.Length;
    public override long Position { get => sourceStream.Position; set => sourceStream.Position = value; }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;

        while (totalBytesRead < count)
        {
            int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
            if (bytesRead == 0)
            {
                if (sourceStream.Position == 0 || !EnableLooping) break; // something wrong with the source stream
                sourceStream.Position = 0; // loop
            }
            totalBytesRead += bytesRead;
        }
        return totalBytesRead;
    }

    protected override void Dispose(bool disposing)
    {
        sourceStream.Dispose();
        base.Dispose(disposing);
    }
}