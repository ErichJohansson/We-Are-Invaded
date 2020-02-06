using System.Collections;
using UnityEngine;

public class StartGameScreen : MonoBehaviour
{
    public UIObject panel;
    public GameObject exitQuestion;
    private GameController gc;

    private void Start()
    {
        gc = FindObjectOfType<GameController>();        
    }

    public void StartGame()
    {
        gc.StartGame();
        gc.pc.GenerateWorld();

        gc.SetPause(!panel.isShown);
        panel.HidePanel();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitQuestion.activeSelf)
            {
                CancelExit();
                return;
            }
            if (panel.isShown)
            {
                ActivateExitQuestion(true);
            }
        }       
    }

    public void ActivateExitQuestion(bool state)
    {
        exitQuestion.SetActive(state);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CancelExit()
    {
        ActivateExitQuestion(false);
    }

    public void ActivateMainMenu()
    {
        gc.SetPause(!panel.isShown);
        panel.ShowPanel();
    }
}