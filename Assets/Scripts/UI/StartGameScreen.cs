using UnityEngine;

namespace UI
{
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
            GameController.Instance.StartGame();
            GameController.Instance.UpdateCash();
            GameController.Instance.SetPause(!panel.isShown);
            UIController.Instance.HidePanel(panel);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && panel.isShown)
            {
                if (exitQuestion.activeSelf)
                {
                    ActivateExitQuestion(false);
                    return;
                }
                if (!panel.shownOnThisFrame)
                    ActivateExitQuestion(true);
                else
                    panel.shownOnThisFrame = false;
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

        public void ActivateMainMenu()
        {
            GameController.Instance.SetPause(!panel.isShown);
            panel.ShowPanel();
        }
    }
}