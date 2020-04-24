using UnityEngine;

namespace UI
{
    public class PauseScreen : MonoBehaviour
    {
        public UIObject panel;
        private ObjectPooler pooler;

        private void Awake()
        {
            pooler = FindObjectOfType<ObjectPooler>();
        }

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
            pooler.Restart();
            panel.HidePanel();
            UIController.Instance.startGameScreen.panel.ShowPanel();
            GameController.Instance.Pause = false;
        }
    }
}
