using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleShowcase : MonoBehaviour
{
    public Vehicle vehicle;
    public Text vehicleName;
    public Image vehicleImage;
    public Text price;

    public Slider damage;
    public Slider health;
    public Slider maxSpeed;
    public Slider turning;
    public Slider reloadTime;

    public Button selectVehicleButton;
    public GameObject colorSchemeScrollView;

    [Header("Question settings")]
    public GameObject question;
    public Button yesButton;
    public Button noButton;

    private ColorShowcase[] colorShowcases;

    [SerializeField]
    private Sprite lockedVahicle;
    [Header("Price tag colors")]
    [SerializeField]
    private Color notEnoughMoneyColor;
    [SerializeField]
    private Color enoughMoneyColor;

    private void Awake()
    {
        colorShowcases = GetComponentsInChildren<ColorShowcase>();
        vehicle = Instantiate(vehicle);
        for (int i = 0; i < vehicle.colorSchemes.Length; i++)
        {
            vehicle.colorSchemes[i] = Instantiate(vehicle.colorSchemes[i]);
        }
    }

    public void ShowVehicle()
    {
        vehicleName.text = vehicle.name;
        vehicleImage.sprite = vehicle.purchased ? vehicle.colorSchemes[vehicle.selectedColorScheme].previewSprite : lockedVahicle;

        damage.value = vehicle.damage / Vehicle.maxDamage;
        health.value = vehicle.health / Vehicle.maxHealth;
        maxSpeed.value = vehicle.speed / Vehicle.maxSpeed;
        turning.value = vehicle.turning / Vehicle.maxTurning;
        reloadTime.value = Vehicle.minReload / vehicle.reloadTime;

        price.text = vehicle.purchased ? vehicle.currentLevel + 1 < vehicle.upgradeLevels.Length ? vehicle.upgradeLevels[vehicle.currentLevel + 1].upgradeCost.ToString() : "" : vehicle.price.ToString();
        
        if (!vehicle.purchased)
            price.color = vehicle.price <= GameController.Instance.cash ? enoughMoneyColor : notEnoughMoneyColor;
        else if (vehicle.currentLevel + 1 < vehicle.upgradeLevels.Length)
            price.color = vehicle.upgradeLevels[vehicle.currentLevel + 1].upgradeCost <= GameController.Instance.cash ? enoughMoneyColor : notEnoughMoneyColor;
        
        selectVehicleButton.interactable = vehicle.purchased;
        foreach (ColorShowcase cs in colorShowcases)
            cs.ShowScheme();
        if(VehicleSelectionController.Instance.SelectedVehicle != null)
            colorSchemeScrollView.SetActive(VehicleSelectionController.Instance.SelectedVehicle.id == vehicle.id);
    }

    #region Question
    public void AskQuestion()
    {
        if (!VehicleSelectionController.Instance.RaisePurchaseVehicleQuestion(vehicle))
            return;
        question.SetActive(true);
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);
    }

    public void No()
    {
        question.SetActive(false);
    }

    public void Yes()
    {
        question.SetActive(false);
        VehicleSelectionController.Instance.PurchaseVehicle(vehicle);
    }
    #endregion
}