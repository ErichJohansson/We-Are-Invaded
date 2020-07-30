using UnityEngine;

namespace UI
{
    public class VehicleSelectionScreen : MonoBehaviour
    {
        public UIObject panel;

        public void ShowScreen()
        {
            GameController.Instance.SomeScreenIsShown = true;
            UIController.Instance.startGameScreen.panel.HidePanel();
            panel.ShowPanel();
        }

        public void BackToMainMenu()
        {
            panel.HidePanel();
            for (int i = 0; i < VehicleSelectionController.Instance.vehicleShowcases.Length; i++)
            {
                VehicleSelectionController.Instance.vehicleShowcases[i].question.SetActive(false);
                VehicleSelectionController.Instance.vehicleShowcases[i].purchaseButton.gameObject.SetActive(true);
                VehicleSelectionController.Instance.vehicleShowcases[i].priceTip.gameObject.SetActive(true);
            }
            UIController.Instance.startGameScreen.panel.ShowPanel();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && panel.isShown)
                BackToMainMenu();
        }
    }
}