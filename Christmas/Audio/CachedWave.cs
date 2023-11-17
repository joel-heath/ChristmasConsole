using NAudio.Wave;

namespace Christmas.Audio;
class CachedWave
{
    public byte[] AudioData { get; private set; }
    public WaveFormat WaveFormat { get; private set; }
    public long Length { get; private set; }
    public CachedWave(string audioFileName)
    {
        using var waveFileReader = new WaveFileReader(audioFileName);
        // TODO: could add resampling in here if required
        WaveFormat = waveFileReader.WaveFormat;
        var wholeFile = new List<byte>((int)(waveFileReader.Length / 4));
        var readBuffer = new byte[waveFileReader.WaveFormat.SampleRate * waveFileReader.WaveFormat.Channels];
        int samplesRead;
        while ((samplesRead = waveFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
        {
            wholeFile.AddRange(readBuffer.Take(samplesRead));
        }
        AudioData = [.. wholeFile];
    }
}