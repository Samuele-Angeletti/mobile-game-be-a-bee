using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderSoundController : MonoBehaviour
{
    [SerializeField] string mixerGroupName;
    [SerializeField] SoundManager soundManager;
    Slider _slider;
    public float LastValue;
    private void Awake()
    {
        _slider = GetComponent<Slider>();
        LastValue = _slider.value;
    }
    public void SetVolume()
    {
        soundManager.SetVolume(mixerGroupName, Mathf.Log10(_slider.value) * 20);
        LastValue = _slider.value;
        if (_slider.value <= 0.01)
            soundManager.SetVolume(mixerGroupName, -80);
    }

    private void OnEnable()
    {
        if (SoundManager.IsMuted)
        {
            var lv = LastValue;
            _slider.value = 0;
            LastValue = lv;
        }
        else
            _slider.value = LastValue == 0 ? 1 : LastValue;
    }
}
