using System.Collections;
using UnityEngine;

public class StartGameScreen : MonoBehaviour
{
    public UIObject panel;
    public GameObject exitQuestion;

    private ObjectPooler pooler;

    private void Awake()
    {
        pooler = FindObjectOfType<ObjectPooler>();
    }

    public void StartGame()
    {
        pooler.Restart();
        GameController.Instance.UpdateCash();
        GameController.Instance.SetPause(!panel.isShown);
        GameController.Instance.StartGame();
        UIController.Instance.HidePanel(panel);
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
        GameController.Instance.SetPause(!panel.isShown);
        panel.ShowPanel();
    }
}