using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleLocalization;

namespace UI
{
    public class GameOverScreen : MonoBehaviour
    {
        public Text info;
        public UIObject panel;

        public AnimatedNumericText traveled;
        public AnimatedNumericText earned;

        private ObjectPooler pooler;
        private Color screenColor;

        private void Awake()
        {
            pooler = FindObjectOfType<ObjectPooler>();
            screenColor = panel.GetComponent<Image>().color;
            screenColor.a = 1;
        }

        private void SetEndGameInfo()
        {
            traveled.SetValue(GameController.Instance.DistanceTraveled);
            earned.SetValue(GameController.Instance.ScoredPoints);
        }

        public void ShowGameOverScreen()
        {
            GameController.Instance.SetPause(!panel.isShown);
            SetEndGameInfo();
            UIController.Instance.ShowPanel(panel);
            UIController.Instance.ChangeLoadEffectColor(screenColor);
            UIController.Instance.ActivateLoadEffect(true);
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
}
