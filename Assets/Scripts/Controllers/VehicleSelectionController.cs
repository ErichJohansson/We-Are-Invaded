using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSelectionController : MonoBehaviour
{
    public Image vehicleImage;
    public Text damage;
    public Text health;
    public Text armor;
    public Text maxSpeed;
    public Text turning;
    public Text reloadTime;
    public Text price;
    public Button selectVehicleButton;

    public Vehicle[] vehicles;

    [SerializeField]
    private Sprite lockedVahicle;

    [Header("Price tag colors")]
    [SerializeField]
    private Color notEnoughMoneyColor;
    [SerializeField]
    private Color enoughMoneyColor;

    private GameObject playerObject;
    private GameController gc;
    private Vehicle selectedVehicle;
    private int vehicleUpgradeCost;

    void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    public void ShowVehicle(Vehicle vehicle)
    {
        selectedVehicle = vehicle;
        vehicleImage.sprite = vehicle.purchased ? vehicle.colorSchemes[vehicle.selectedColorScheme] : lockedVahicle;
        damage.text = vehicle.damage.ToString();
        health.text = vehicle.health.ToString();
        //armor.text = vehicle.armor.ToString();
        maxSpeed.text = vehicle.speed.ToString();
        turning.text = vehicle.turning.ToString();
        reloadTime.text = vehicle.reloadTime.ToString();
        playerObject = vehicle.playerObject;

        price.text = vehicle.purchased ? "" : vehicle.price.ToString();
        price.color = gc.cash >= vehicle.price ? enoughMoneyColor : notEnoughMoneyColor;
        selectVehicleButton.interactable = vehicle.purchased;
    }

    /// <summary>
    /// Call on APPLY button click.
    /// </summary>
    public void SelectVehicle()
    {
        if (gc.playerObject != null)
            Destroy(gc.playerObject);
        gc.pc.SpawnPlayer(playerObject, selectedVehicle);
    }

    public void SelectVehicle(int id)
    {
        ShowVehicle(vehicles[id]);
        if (id >= vehicles.Length)
            return;
        if (gc.playerObject != null)
            Destroy(gc.playerObject);
        gc.pc.SpawnPlayer(playerObject, selectedVehicle);
    }

    public void SelectVehicle(Vehicle vehicle)
    {
        ShowVehicle(vehicle);

        if (gc.playerObject != null)
            Destroy(gc.playerObject);
        gc.pc.SpawnPlayer(playerObject, vehicle);
    }

    public void PurchaseVehicle()
    {
        if (selectedVehicle == null || selectedVehicle.purchased)
            return;

        if(gc.cash >= selectedVehicle.price)
        {
            gc.cash -= selectedVehicle.price;
            selectedVehicle.purchased = true;
            gc.SaveGame();

            ShowVehicle(selectedVehicle);
            gc.uc.UpdateCash();
        }
    }

    public void UpgradeSelectedVehilce()
    {
        if (selectedVehicle == null || gc.cash < vehicleUpgradeCost)
            return;
        gc.cash -= vehicleUpgradeCost;
        /*
         Upgrade vehicle stats
         recalculate vehicleUpgradeCost
         Update UI
         Respawn player / or update player stats

        TODO: add UI elements for upgrading and picking color scheme
        */
        gc.SaveGame();
    }
}
