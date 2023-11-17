using NAudio.Wave;

namespace Christmas.Audio;
class CachedSound
{
    public float[] AudioData { get; private set; }
    public WaveFormat WaveFormat { get; private set; }
    public CachedSound(string audioFileName)
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