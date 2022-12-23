using UnityEngine;

public interface ISoundMaker
{
    public AudioSource AudioSource { get; set; }
    public string MixerFatherName { get; set; }
    public void PlaySound();
    public void StopSound();
}
