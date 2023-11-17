using NAudio.Wave;

namespace Christmas.Audio;
class CachedWaveStream(CachedWave cachedSound) : WaveStream
{
    private readonly CachedWave cachedSound = cachedSound;
    public override long Position { get; set; }
    public override long Length => cachedSound.Length;
    public override WaveFormat WaveFormat => cachedSound.WaveFormat;

    public override int Read(byte[] buffer, int offset, int count)
    {
        var availableSamples = cachedSound.AudioData.Length - Position;
        var samplesToCopy = Math.Min(availableSamples, count);
        Array.Copy(cachedSound.AudioData, Position, buffer, offset, samplesToCopy);
        Position += samplesToCopy;
        return (int)samplesToCopy;
    }
}