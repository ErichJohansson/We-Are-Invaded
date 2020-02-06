using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleLocalization;

public class GameOverScreen : MonoBehaviour
{
    public Text info;
    public UIObject panel;
    private GameController gc;

    private void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    private void SetEndGameInfo(string gameinfo)
    {
        info.text = gameinfo;
    }

    public void ShowGameOverScreen()
    {
        gc.SetPause(!panel.isShown);
        SetEndGameInfo(LocalizationManager.Localize("GameOver.GameInfo", gc.ScoredPoints.ToString(), gc.DistanceTraveled.ToString()));
        gc.uc.ShowPanel(panel);

        gc.SaveGame();
    }

    public void BackToMainMenu()
    {
        gc.UpdateCash();
        panel.HidePanel();
        gc.uc.startGameScreen.panel.ShowPanel();
    }

    public void RestartGame()
    {
        gc.UpdateCash();
        gc.SetPause(!panel.isShown);
        gc.StartGame();
        gc.uc.HidePanel(panel);
    }
}
