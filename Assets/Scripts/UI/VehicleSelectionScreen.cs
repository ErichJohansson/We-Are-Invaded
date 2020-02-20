using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSelectionScreen : MonoBehaviour
{
    public UIObject panel;

    public void ShowScreen()
    {
        GameController.Instance.SomeScreenIsShown = true;
        UIController.Instance.startGameScreen.panel.HidePanel();
        panel.ShowPanel();
    }

    public void BackToMainMenu()
    {
        panel.HidePanel();
        UIController.Instance.startGameScreen.panel.ShowPanel();
    }
}
