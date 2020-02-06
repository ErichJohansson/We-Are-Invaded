using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public UIObject panel;
    private GameController gc;

    private void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    private void ResumeGame() 
    {
        gc.SetPause(false);
        panel.HidePanel();
    }

    private void PauseGame()
    {
        panel.ShowPanel();
        gc.SetPause(true);
    }

    public void ActivatePause()
    {
        if (gc.someScreenIsShown && !panel.isShown)
            return;
        if (Time.timeScale == 0)
            ResumeGame();
        else
            PauseGame();
    }

    public void BackToMainMenu()
    {
        panel.HidePanel();
        gc.uc.startGameScreen.panel.ShowPanel();
        gc.Pause = false;
    }
}
