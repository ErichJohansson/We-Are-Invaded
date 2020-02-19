using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleShowcase : MonoBehaviour
{
    public Vehicle vehicle;
    public Image vehicleImage;
    public Text damage;
    public Text health;
    public Text maxSpeed;
    public Text turning;
    public Text reloadTime;
    public Text price;
    public Button selectVehicleButton;
    public GameObject colorSchemeScrollView;

    public ColorShowcase[] colorShowcases;

    [SerializeField]
    private Sprite lockedVahicle;
    [Header("Price tag colors")]
    [SerializeField]
    private Color notEnoughMoneyColor;
    [SerializeField]
    private Color enoughMoneyColor;

    private GameController gc;

    private void Awake()
    {
        gc = FindObjectOfType<GameController>();
    }

    public void ShowVehicle()
    {
        vehicleImage.sprite = vehicle.purchased ? vehicle.colorSchemes[vehicle.selectedColorScheme].previewSprite : lockedVahicle;
        damage.text = vehicle.damage.ToString();
        health.text = vehicle.health.ToString();
        maxSpeed.text = vehicle.speed.ToString();
        turning.text = vehicle.turning.ToString();
        reloadTime.text = vehicle.reloadTime.ToString();

        price.text = vehicle.purchased ? vehicle.currentLevel + 1 < vehicle.upgradeLevels.Length ? vehicle.upgradeLevels[vehicle.currentLevel + 1].upgradeCost.ToString() : "" : vehicle.price.ToString();
        
        if (!vehicle.purchased)
            price.color = vehicle.price <= gc.cash ? enoughMoneyColor : notEnoughMoneyColor;
        else if (vehicle.currentLevel + 1 < vehicle.upgradeLevels.Length)
            price.color = vehicle.upgradeLevels[vehicle.currentLevel + 1].upgradeCost <= gc.cash ? enoughMoneyColor : notEnoughMoneyColor;
        
        selectVehicleButton.interactable = vehicle.purchased;
        foreach (ColorShowcase cs in colorShowcases)
            cs.ShowScheme();
        if(gc.vehicleSelection.SelectedVehicle != null)
            colorSchemeScrollView.SetActive(gc.vehicleSelection.SelectedVehicle.id == vehicle.id);
    }

    public void BuyOrUpgrade()
    {
        gc.vehicleSelection.PurchaseVehicle(vehicle);
    }
}
