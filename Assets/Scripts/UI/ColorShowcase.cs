using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ColorShowcase : MonoBehaviour
    {
        public Text priceTag;
        public VehicleShowcase showcase;
        public int id;

        [Header("Price tag colors")]
        [SerializeField]
        private Color notEnoughMoneyColor;
        [SerializeField]
        private Color enoughMoneyColor;

        [Header("Question settings")]
        public GameObject question;
        public Button yesButton;
        public Button noButton;

        private void Awake()
        {
            VehicleSelectionController.Instance.scrollSnap.PropertyChanged += OnVehicleViewed;
        }

        private void OnVehicleViewed(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            No();
        }

        public void ShowScheme()
        {
            priceTag.text = showcase.vehicle.colorSchemes[id].price.ToString();
            UpdatePriceColor();
            priceTag.gameObject.SetActive(!showcase.vehicle.colorSchemes[id].purchased);
        }

        #region Question
        public void AskQuestion()
        {
            if (showcase.vehicle.colorSchemes[id].purchased)
            {
                VehicleSelectionController.Instance.ActivateColorSheme(showcase.vehicle, id);
                return;
            }
            if (!VehicleSelectionController.Instance.RaiseActivateColorShemeQuestion(showcase.vehicle, id))
                return;
            question.SetActive(true);
            showcase.purchaseButton.gameObject.SetActive(false);
            showcase.priceTip.gameObject.SetActive(false);
            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(Yes);
            noButton.onClick.AddListener(No);
        }

        public void No()
        {
            showcase.priceTip.gameObject.SetActive(true);
            showcase.purchaseButton.gameObject.SetActive(true);
            question.SetActive(false);
        }

        public void Yes()
        {
            if (VehicleSelectionController.Instance.ActivateColorSheme(showcase.vehicle, id))
            {
                priceTag.gameObject.SetActive(false);
            }
            question.SetActive(false);
        }
        #endregion

        public void UpdatePriceColor()
        {
            priceTag.color = showcase.vehicle.colorSchemes[id].price <= GameController.Instance.cash ? enoughMoneyColor : notEnoughMoneyColor;
        }
    }
}
