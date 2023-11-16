using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Christmas.Audio;
class AudioEngine : IDisposable
{
    private readonly IWavePlayer outputDevice;
    private readonly MixingSampleProvider mixer;
    //private LoopStream? loopingMusic;
    private ISampleProvider? loopingMusic;
    private ISampleProvider? loopingMixerInput;

    public AudioEngine(int sampleRate = 44100, int channelCount = 2)
    {
        var filter = BiQuadFilter.LowPassFilter(44100, 1500, 1);

        outputDevice = new WaveOutEvent();
        mixer = new(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount)) { ReadFully = true };
        outputDevice.Init(mixer);
        outputDevice.Play();
    }

    private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
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

    public void PlaySound(CachedSound sound, bool wait = false)
    {
        var input = AddMixerInput(new CachedSoundSampleProvider(sound));
        if (wait)
        {
            while (mixer.MixerInputs.Contains(input))
            {
                Thread.Sleep(500);
            }
        }
    }

    private ISampleProvider AddMixerInput(ISampleProvider input)
    {
        var mixerInput = ConvertToRightChannelCount(input);
        mixer.AddMixerInput(mixerInput);
        return mixerInput;
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
            //loopingMusic?.Dispose();
            loopingMusic = null;
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

        loopingMusic = new LoopStream(audioLocation).ToSampleProvider();

        loopingMixerInput = AddMixerInput(loopingMusic);
    }

    public void StopAllSounds()
    {
        StopLoopingMusic();
        mixer.RemoveAllMixerInputs();
    }

    public void EnableLPF(int cutoff = 1000)
    {
        if (loopingMusic is null) return;

        loopingMusic = new FilterStream(loopingMusic, cutoff);
        mixer.RemoveMixerInput(loopingMixerInput);
        loopingMixerInput = AddMixerInput(loopingMusic);
    }

    public void DisableLPF()
    {
        loopingMusic = (loopingMusic as FilterStream)?.SourceProvider;

        if (loopingMusic is null) return;
        
        mixer.RemoveMixerInput(loopingMixerInput);
        loopingMixerInput = AddMixerInput(loopingMusic);
    }

    public static readonly AudioEngine Instance = new(44100, 2);
}