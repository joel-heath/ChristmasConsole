using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Diagnostics.CodeAnalysis;

namespace Christmas.Audio;
public class MusicNotPlayingException : Exception { }
class AudioEngine : IDisposable
{
    private readonly WaveOutEvent outputDevice;
    private readonly MixingSampleProvider mixer;

    private WaveStream? sourceStream;
    private ISampleProvider? filter;
    private VolumeSampleProvider? loopingMixerInput;

    private float musicVolume = 1;

    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = value;
            if (loopingMixerInput is not null)
            {
                loopingMixerInput.Volume = value;
            }
        }
    }
    public float SoundsVolume { get; set; } = 1;
    public AudioEngine(int sampleRate = 44100, int channelCount = 2)
    {
        outputDevice = new WaveOutEvent(); // { DesiredLatency = 300 };
        mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount)) { ReadFully = true };
        outputDevice.Init(mixer);
        outputDevice.Play();
    }

    private ISampleProvider ConvertToStereo(ISampleProvider input)
    {
        if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
        {
            return input;
        }
        if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
        {
            return new MonoToStereoSampleProvider(input);
        }
        throw new NotImplementedException("Not yet implemented this channel count conversion");
    }

    public void PlaySound(CachedWave sound, bool wait = false)
    {
        var input = ConvertToStereo(new VolumeSampleProvider(new CachedWaveStream(sound).ToSampleProvider(), SoundsVolume));
        AddMixerInput(input);
        if (wait)
        {
            while (mixer.MixerInputs.Contains(input))
            {
                Thread.Sleep(500);
            }
        }
    }

    private void AddMixerInput(ISampleProvider input)
    {
        mixer.AddMixerInput(input);
    }

    public void Dispose()
    {
        outputDevice.Dispose();
    }

    public void StopLoopingMusic()
    {
        if (loopingMixerInput != null)
        {
            mixer.RemoveMixerInput(loopingMixerInput);
            loopingMixerInput = null;
            sourceStream?.Dispose();
            filter = null;
        }
    }

    public void PauseLoopingMusic()
    {
        if (loopingMixerInput != null)
        {
            mixer.RemoveMixerInput(loopingMixerInput);
        }
    }

    public void ResumeLoopingMusic()
    {
        if (loopingMixerInput != null)
        {
            mixer.AddMixerInput(loopingMixerInput);
        }
    }

    public void PlayLoopingMusic(string audioLocation)
    {
        StopLoopingMusic();
        sourceStream = new LoopStream(audioLocation);
        filter = new FilterSampleProvider(sourceStream.ToSampleProvider(), 1000, true);
        loopingMixerInput = new VolumeSampleProvider(filter, musicVolume);
        AddMixerInput(loopingMixerInput);
    }

    public void StopAllSounds()
    {
        StopLoopingMusic();
        mixer.RemoveAllMixerInputs();
    }

    public void EnableLPF(int cutoff = 1000)
    {
        if (filter is not FilterSampleProvider music) return;
        music.Cutoff = cutoff;
        music.Bypass = false;
    }

    public void DisableLPF()
    {
        if (filter is not FilterSampleProvider music) return;
        music.Bypass = true;
    }

    public static readonly AudioEngine Instance = new(44100, 2);
}