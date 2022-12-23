using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour, ISoundMaker
{
    #region SINGLETON
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
                if (instance != null)
                    return instance;

                GameObject go = new GameObject("GameManager");
                return go.AddComponent<SoundManager>();
            }
            else
                return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    [SerializeField] AudioMixer audioMixer;
    [Header("Mixer Group Names")]
    [SerializeField] string masterName;
    [SerializeField] string effectsName;
    [SerializeField] string flapName;
    [SerializeField] string musicName;
    [SerializeField] string shopName;
    [SerializeField] string gameName;
    [SerializeField] string uiButtonsName;
    [SerializeField] string mainMenuName;
    [Header("Global Sounds")]
    [SerializeField] AudioClip normalUIButton;
    [SerializeField] AudioClip backUIButton;
    [SerializeField] UISliderSoundController effectSlider;
    [SerializeField] UISliderSoundController musicSlider;
    public static bool IsMuted;
    public AudioSource AudioSource { get; set; }
    public string MixerFatherName { get; set; }

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        MixerFatherName = GetMixerFatherName(AudioSource.outputAudioMixerGroup.name);
        
    }
    public void SetVolume(string mixerGroupName, float value)
    {
        audioMixer.SetFloat(mixerGroupName + "Volume", value);

        audioMixer.GetFloat(effectsName + "Volume", out var effVal);
        audioMixer.GetFloat(musicName + "Volume", out var musVal);

        IsMuted = effVal == -80 && musVal == -80;
    }

    public void Mute(bool mute)
    {
        if (mute)
        {
            SetVolume(effectsName, -80);
            SetVolume(musicName, -80);
        }
        else
        {
            SetVolume(effectsName, effectSlider.LastValue);
            SetVolume(musicName, musicSlider.LastValue);
        }

        IsMuted = mute;
    }

    public void PlayNormalButton()
    {
        PlayGlobalSound(normalUIButton);
    }

    public void PlayBackButton()
    {
        PlayGlobalSound(backUIButton);
    }

    private void PlayGlobalSound(AudioClip clip)
    {
        StopSound();

        AudioSource.clip = clip;

        PlaySound();
    }

    public void PlaySound()
    {
        if (!IsMuted && ! IsMixerMuted(MixerFatherName))
            AudioSource.Play();
    }

    public void StopSound()
    {
        AudioSource.Stop();
    }

    public string GetMixerFatherName(string childMixerName)
    {
        if(childMixerName == musicName)
            return musicName;

        if(childMixerName == effectsName)
            return effectsName;

        if (childMixerName == masterName)
            return masterName;

        if (childMixerName == flapName || childMixerName == uiButtonsName)
        {
            return effectsName;
        }

        return musicName;
    }

    public bool IsMixerMuted(string mixerName)
    {
        audioMixer.GetFloat(mixerName + "Volume", out var value);
        return value == -80;
    }
}
