using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonAction : MonoBehaviour
{
    EScenario currentScenario;
    bool goingUp;
    public void ScenarioSelected()
    {
        Publisher.Publish(new ScenarioChoosedMessage(currentScenario, goingUp));
    }

    internal void LoadScenarioOnButton(EScenario scenairo, bool goingUp)
    {
        currentScenario = scenairo;
        this.goingUp = goingUp;
    }
}
