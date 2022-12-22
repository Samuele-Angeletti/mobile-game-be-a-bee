using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioChoosedMessage : IMessage
{
    public ScenarioChoosedMessage(EScenario currentScenario, bool goingUp)
    {
        CurrentScenario = currentScenario;
        GoingUp = goingUp;
    }

    public EScenario CurrentScenario { get; }
    public bool GoingUp { get; }
}
