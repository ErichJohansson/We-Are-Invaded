using Assets.SimpleLocalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class VehicleShowcase : MonoBehaviour
    {
        public Vehicle vehicle;
        public Text vehicleName;
        public Image previewImage;
        public ImageAnimator previewClip;
        public Text price;

        [Header("Price Tip")]
        public LocalizedText priceTipText;
        public Text priceTip;

        [Header("Vehicle Characteristics Sliders")]
        public Slider damage;
        public Slider health;
        public Slider maxSpeed;
        public Slider turning;
        public Slider reloadTime;

        public Slider damageUpgraded;
        public Slider healthUpgraded;
        public Slider maxSpeedUpgraded;
        public Slider turningUpgraded;
        public Slider reloadTimeUpgraded;

        public GameObject vehicleLock;

        public Button purchaseButton;
        public Button selectVehicleButton;
        public GameObject colorSchemeScrollView;

        public GameObject shadow;

        [Header("Question settings")]
        public GameObject question;
        public Button yesButton;
        public Button noButton;

        public ColorShowcase[] colorShowcases;

        [Header("Price tag colors")]
        [SerializeField]
        private Color notEnoughMoneyColor;
        [SerializeField]
        private Color enoughMoneyColor;

        private void Awake()
        {
            if (colorShowcases.Length == 0) colorShowcases = GetComponentsInChildren<ColorShowcase>();
            vehicle = Instantiate(vehicle);
            for (int i = 0; i < vehicle.colorSchemes.Length; i++)
            {
                vehicle.colorSchemes[i] = Instantiate(vehicle.colorSchemes[i]);
            }
        }

        public void ShowVehicle()
        {
            vehicleName.text = vehicle.name;
            previewClip.clip = vehicle.colorSchemes[vehicle.selectedColorScheme].previewClip;
            previewImage.sprite = vehicle.colorSchemes[vehicle.selectedColorScheme].previewSprites[0];
            ImageAnimator ia = GetComponentInChildren<ImageAnimator>();
            if (ia != null)
            {
                ia.clip = vehicle.colorSchemes[vehicle.selectedColorScheme].previewClip;
                ia.sprites = vehicle.colorSchemes[vehicle.selectedColorScheme].previewSprites;
            }
            vehicleLock.SetActive(!vehicle.purchased);
            damage.value = (float)vehicle.damage / (float)Vehicle.maxDamage;
            health.value = (float)vehicle.health / (float)Vehicle.maxHealth;
            maxSpeed.value = vehicle.speed / Vehicle.maxSpeed;
            turning.value = vehicle.turning / Vehicle.maxTurning;
            reloadTime.value = Vehicle.minReload / vehicle.reloadTime;

            price.text = vehicle.purchased ? vehicle.currentLevel + 1 < vehicle.upgradeLevels.Length ? vehicle.upgradeLevels[vehicle.currentLevel + 1].upgradeCost.ToString() : "" : vehicle.price.ToString();
            priceTip.text = vehicle.purchased ? vehicle.currentLevel + 1 < vehicle.upgradeLevels.Length ? "TankSelect.UpgradeAvailable" : "TankSelect.FullUpgrade" : "TankSelect.Purchase";
            priceTipText.Localize();

            if (!vehicle.purchased)
                price.color = vehicle.price <= GameController.Instance.cash ? enoughMoneyColor : notEnoughMoneyColor;
            else if (vehicle.currentLevel + 1 < vehicle.upgradeLevels.Length)
                price.color = vehicle.upgradeLevels[vehicle.currentLevel + 1].upgradeCost <= GameController.Instance.cash ? enoughMoneyColor : notEnoughMoneyColor;

            Vehicle selectedVehicle = VehicleSelectionController.Instance.SelectedVehicle;
            if (selectedVehicle != null && selectedVehicle.id == vehicle.id) selectVehicleButton.interactable = vehicle.purchased;
            if (colorShowcases.Length == 0) colorShowcases = GetComponentsInChildren<ColorShowcase>();
            foreach (ColorShowcase cs in colorShowcases)
                cs.ShowScheme();
            if (VehicleSelectionController.Instance.SelectedVehicle != null)
                colorSchemeScrollView.SetActive(VehicleSelectionController.Instance.SelectedVehicle.id == vehicle.id);
        }

        #region Question
        public void AskQuestion()
        {
            if (!VehicleSelectionController.Instance.RaisePurchaseVehicleQuestion(vehicle))
                return;

            Vehicle upgradedVehicle = Vehicle.GetUpgradedVehicle(vehicle);
            if (upgradedVehicle == null)
                return;

            purchaseButton.gameObject.SetActive(false);
            priceTip.gameObject.SetActive(false);
            if (upgradedVehicle.purchased == true)
            {
                damageUpgraded.value = upgradedVehicle.damage / Vehicle.maxDamage;
                healthUpgraded.value = upgradedVehicle.health / Vehicle.maxHealth;
                maxSpeedUpgraded.value = upgradedVehicle.speed / Vehicle.maxSpeed;
                turningUpgraded.value = upgradedVehicle.turning / Vehicle.maxTurning;
                reloadTimeUpgraded.value = Vehicle.minReload / upgradedVehicle.reloadTime;
            }

            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(Yes);
            noButton.onClick.AddListener(No);

            question.SetActive(true);
        }

        public void No()
        {
            SetUpgradedStatsToZero();
            question.SetActive(false);
            purchaseButton.gameObject.SetActive(true);
            priceTip.gameObject.SetActive(true);
        }

        public void Yes()
        {
            SetUpgradedStatsToZero();
            question.SetActive(false);
            purchaseButton.gameObject.SetActive(true);
            priceTip.gameObject.SetActive(true);
            VehicleSelectionController.Instance.PurchaseVehicle(vehicle);
        }
        #endregion

        private void SetUpgradedStatsToZero()
        {
            damageUpgraded.value = 0;
            healthUpgraded.value = 0;
            maxSpeedUpgraded.value = 0;
            turningUpgraded.value = 0;
            reloadTimeUpgraded.value = 0;
        }
    }
}