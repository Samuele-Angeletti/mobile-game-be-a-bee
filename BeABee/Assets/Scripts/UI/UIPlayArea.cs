
using System;
using System.Collections;
using TMPro;
using UnityEngine;
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

    [Header("Pause Panel")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] Button pauseButton;
    GameManager _gameManager;
    BossCondition _currentCondition;

    [Header("Scenario Choise Buttons")]
    [SerializeField] GameObject scenarioButtonsPanel;
    [SerializeField] Button upDirectionButton;
    [SerializeField] Button middleDirectionButton;
    [SerializeField] Button lowDirectionButton;
    [SerializeField] Slider timerTextSlider;
    [SerializeField] float timer;

    private float timePassed;
    Coroutine _selectingScenario;
    private void Start()
    {
        _gameManager = GameManager.Instance;

        Publisher.Subscribe(this, typeof(BossConditionChangedMessage));
        Publisher.Subscribe(this, typeof(ChoosingNextScenarioMessage));
        Publisher.Subscribe(this, typeof(ScenarioChoosedMessage));
    }

    public void ResetValues()
    {
        meterValue.text = string.Empty;
        scoreValue.text = string.Empty;
        flockValue.text = string.Empty;
        currentFlockValue.text = "x 0";

        bombButton.interactable = false;
        bombQuantityValue.text = "x 0";

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
        if(bombButton.interactable != enabled) 
            bombButton.interactable = enabled;
    }

    private void Update()
    {
        if (_gameManager.IsGamePlaying)
        {
            meterValue.text = $"{(int)_gameManager.MetersDone}";
            scoreValue.text = $"{_gameManager.ScoreDone}";
            flockValue.text = $"{_gameManager.FlockMax}";
            currentFlockValue.text = $"x {_gameManager.CurrentFlock}";

            if(_currentCondition != null)
                ColorConditionValues(_currentCondition);
        }
    }

    public void OnPublish(IMessage message)
    {
        if (message is BossConditionChangedMessage bossConditionMessage)
        {
            var condition = bossConditionMessage.BossCondition;
            if (condition == null)
                return;
            bossNameValue.text = condition.BossPrefab.BossName;
            bossImage.sprite = condition.BossPrefab.UiIcon;
            meterConditionValue.text = $"{(int)condition.Meters}";
            scoreConditionValue.text = $"{condition.Score}";
            flockConditionValue.text = $"{condition.MaxFlockHad}";
            _currentCondition = condition;
            ColorConditionValues(condition);
        }
        else if(message is ChoosingNextScenarioMessage)
        {
            scenarioButtonsPanel.SetActive(true);
            LoadScenarioChoises();
            pauseButton.interactable = false;
            _selectingScenario = StartCoroutine(DisplayButtonsCoroutine());
        }
        else if(message is ScenarioChoosedMessage)
        {
            if (_selectingScenario != null)
            {
                StopCoroutine(_selectingScenario);
                _selectingScenario = null;
            }
            pauseButton.interactable = true;
        }
    }

    private IEnumerator DisplayButtonsCoroutine()
    {
        timePassed = timer;
        timerTextSlider.value = 1;
        while (true)
        {
            timerTextSlider.value = timePassed / timer;
            timePassed -= Time.deltaTime;
            if(timePassed <= 0)
            {
                middleDirectionButton.GetComponent<UIButtonAction>().ScenarioSelected();
                scenarioButtonsPanel.SetActive(false);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        _selectingScenario = null;
        yield return null;
    }

    private void ColorConditionValues(BossCondition condition)
    {
        meterConditionValue.color = condition.Meters > _gameManager.MetersDone ? Color.red : Color.white;
        scoreConditionValue.color = condition.Score > _gameManager.ScoreDone ? Color.red : Color.white;
        flockConditionValue.color = condition.MaxFlockHad > _gameManager.FlockMax ? Color.red : Color.white;
    }

    private void OnDestroy()
    {
        Publisher.Unsubscribe(this, typeof(BossConditionChangedMessage));
        Publisher.Unsubscribe(this, typeof(ChoosingNextScenarioMessage));
        Publisher.Unsubscribe(this, typeof(ScenarioChoosedMessage));
    }

    public void HandlePausePanel()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
    }

    // TODO: _currentScenario >> calcolate next nearby scenarios >> place scenarios on buttons with also the sprite of the next scenarios
    public void LoadScenarioChoises()
    {
        upDirectionButton.GetComponent<UIButtonAction>().LoadScenarioOnButton();
        middleDirectionButton.GetComponent<UIButtonAction>().LoadScenarioOnButton();
        lowDirectionButton.GetComponent<UIButtonAction>().LoadScenarioOnButton();
    }


}
