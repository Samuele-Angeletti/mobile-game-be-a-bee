using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIPlayArea : MonoBehaviour, ISubscriber
{
    [Header("Game Values")]
    [SerializeField] TextMeshProUGUI meterValue;
    [SerializeField] TextMeshProUGUI flockValue;
    [SerializeField] TextMeshProUGUI scoreValue;
    [SerializeField] TextMeshProUGUI currentFlockValue;

    [Header("Bomb Values")]
    [SerializeField] Button bombButton;
    [SerializeField] TextMeshProUGUI bombQuantityValue;

    [Header("Boss Values")]
    [SerializeField] TextMeshProUGUI bossNameValue;
    [SerializeField] Image bossImage;
    [SerializeField] TextMeshProUGUI meterConditionValue;
    [SerializeField] TextMeshProUGUI flockConditionValue;
    [SerializeField] TextMeshProUGUI scoreConditionValue;

    GameManager _gameManager;
    private void Start()
    {
        _gameManager = GameManager.Instance;

        Publisher.Subscribe(this, typeof(BossConditionChangedMessage));
    }

    public void ResetValues()
    {
        meterValue.text = string.Empty;
        scoreValue.text = string.Empty;
        flockValue.text = string.Empty;
        currentFlockValue.text = string.Empty;

        bombButton.interactable = false;
        bombQuantityValue.text = string.Empty;

        bossNameValue.text = string.Empty;
        meterConditionValue.text = string.Empty;
        flockConditionValue.text = string.Empty;
        scoreConditionValue.text = string.Empty;

        bossImage.sprite = null;
    }

    public void UpdateBombQuantity(int amount)
    {
        bombQuantityValue.text = $"x {amount}";
    }

    public void EnableBombButton(bool enabled)
    {
        bombButton.interactable = enabled;
    }

    private void Update()
    {
        if(_gameManager.IsGamePlaying)
        {
            meterValue.text = $"{(int)_gameManager.MetersDone}";
            scoreValue.text = $"{_gameManager.ScoreDone}";
            flockValue.text = $"{_gameManager.FlockMax}";
            currentFlockValue.text = $"{_gameManager.CurrentFlock}";
        }
    }

    public void OnPublish(IMessage message)
    {
        if(message is BossConditionChangedMessage bossConditionMessage)
        {
            var condition = bossConditionMessage.BossCondition;
            if (condition == null)
                return;
            bossNameValue.text = condition.BossPrefab.BossName;
            bossImage.sprite = condition.BossPrefab.UiIcon;
            meterConditionValue.text = $"{(int)condition.Meters}";
            scoreConditionValue.text = $"{condition.Score}";
            flockConditionValue.text = $"{condition.MaxFlockHad}";
        }
    }

    private void OnDestroy()
    {
        Publisher.Unsubscribe(this, typeof(BossConditionChangedMessage));
    }
}
