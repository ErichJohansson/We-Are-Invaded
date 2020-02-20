using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public UIObject panel;

    private void ResumeGame() 
    {
        GameController.Instance.SetPause(false);
        panel.HidePanel();
    }

    private void PauseGame()
    {
        panel.ShowPanel();
        GameController.Instance.SetPause(true);
    }

    public void ActivatePause()
    {
        if (GameController.Instance.SomeScreenIsShown && !panel.isShown)
            return;
        if (Time.timeScale == 0)
            ResumeGame();
        else
            PauseGame();
    }

    public void BackToMainMenu()
    {
        panel.HidePanel();
        UIController.Instance.startGameScreen.panel.ShowPanel();
        GameController.Instance.Pause = false;
    }
}
