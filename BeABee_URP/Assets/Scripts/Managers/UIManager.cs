using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, ISoundMaker
{
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject ShopMenu;
    [SerializeField] GameObject PlayArea;
    [SerializeField] GameObject GameOverMenu;
    private UIGameOver _uiGameOver;
    private UIPlayArea _uiPlayArea;
    public string MixerFatherName { get; set; }
    public AudioSource AudioSource { get ; set ; }

    private void Awake()
    {
        _uiPlayArea = PlayArea.SearchComponent<UIPlayArea>();
        _uiGameOver = GameOverMenu.SearchComponent<UIGameOver>();
        AudioSource = GetComponent<AudioSource>();
        MixerFatherName = SoundManager.Instance.GetMixerFatherName(AudioSource.outputAudioMixerGroup.name);
    }

    private void Start()
    {
        GameManager.Instance.onGameOver += () => PlaySound();
    }

    public void ResetMenu()
    {
        MainMenu.SetActive(true);
        ShopMenu.SetActive(false);
        PlayArea.SetActive(false);
        GameOverMenu.SetActive(false);

        _uiPlayArea.ResetValues();
    }

    public void ShowFinalStats()
    {
        _uiGameOver.FillUpStatistics();
        GameOverMenu.SetActive(true);
        PlayArea.SetActive(false);
    }

    public void PlaySound()
    {
        if (!SoundManager.IsMuted && !SoundManager.Instance.IsMixerMuted(MixerFatherName))
            AudioSource.Play();
    }

    public void StopSound()
    {
        AudioSource.Stop();
    }
}
