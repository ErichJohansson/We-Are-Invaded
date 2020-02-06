using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSelectionScreen : MonoBehaviour
{
    public UIObject panel;
    private GameController gc;

    private void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    public void ShowScreen()
    {
        gc.someScreenIsShown = true;
        gc.uc.startGameScreen.panel.HidePanel();
        panel.ShowPanel();
    }

    public void BackToMainMenu()
    {
        panel.HidePanel();
        gc.uc.startGameScreen.panel.ShowPanel();
    }
}
