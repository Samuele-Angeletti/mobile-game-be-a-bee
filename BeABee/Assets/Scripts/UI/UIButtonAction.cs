using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonAction : MonoBehaviour
{
    public void LoadScenarioOnButton()
    {

    }

    public void ScenarioSelected()
    {
        Publisher.Publish(new ScenarioChoosedMessage());
    }
}
