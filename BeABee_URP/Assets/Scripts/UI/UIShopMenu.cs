using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIShopMenu : MonoBehaviour, ISoundMaker
{
    [SerializeField] TextMeshProUGUI totalMetersText;
    [SerializeField] TextMeshProUGUI totalHoneyText;

    public AudioSource AudioSource { get; set; }
    public string MixerFatherName { get; set ; }

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        MixerFatherName = SoundManager.Instance.GetMixerFatherName(AudioSource.outputAudioMixerGroup.name);
    }
    public void LoadStatistics()
    {
        totalMetersText.text = $"{PlayerStatistics.TotalMeters}";
        totalHoneyText.text = $"{PlayerStatistics.TotalHoney}";
    }

    public void PlaySound()
    {
        if(!SoundManager.IsMuted && !SoundManager.Instance.IsMixerMuted(MixerFatherName))
            AudioSource.Play();
    }

    public void StopSound()
    {
        AudioSource.Stop();
    }
}
