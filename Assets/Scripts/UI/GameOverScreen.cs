using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleLocalization;

public class GameOverScreen : MonoBehaviour
{
    public Text info;
    public UIObject panel;

    public AnimatedNumericText traveled;
    public AnimatedNumericText earned;

    private ObjectPooler pooler;

    private void Awake()
    {
        pooler = FindObjectOfType<ObjectPooler>();
    }

    private void SetEndGameInfo()
    {
        traveled.SetValue(GameController.Instance.DistanceTraveled);
        earned.SetValue(GameController.Instance.ScoredPoints);
    }

    public void ShowGameOverScreen()
    {
        GameController.Instance.SetPause(!panel.isShown);
        //SetEndGameInfo(LocalizationManager.Localize("GameOver.GameInfo", GameController.Instance.ScoredPoints.ToString(), GameController.Instance.DistanceTraveled.ToString()));
        SetEndGameInfo();
        UIController.Instance.ShowPanel(panel);

        GameController.Instance.SaveGame();
    }

    public void BackToMainMenu()
    {
        GameController.Instance.UpdateCash();
        panel.HidePanel();
        UIController.Instance.startGameScreen.panel.ShowPanel();
    }

    public void RestartGame()
    {
        pooler.Restart();
        GameController.Instance.UpdateCash();
        GameController.Instance.SetPause(!panel.isShown);
        GameController.Instance.StartGame();
        UIController.Instance.HidePanel(panel);
    }
}
