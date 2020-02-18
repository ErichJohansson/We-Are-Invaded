using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSelectionController : MonoBehaviour
{
    public VehicleShowcase[] vehicles;
    public GameObject selectVehicleButton;

    private GameObject playerObject;
    private GameController gc;
    private Vehicle selectedVehicle;
    private ScrollSnap scrollSnap;

    void Awake()
    {
        gc = FindObjectOfType<GameController>();
        scrollSnap = FindObjectOfType<ScrollSnap>();
        scrollSnap.PropertyChanged += PropertyChangedHandler;
    }

    /// <summary>
    /// Call on APPLY button click.
    /// </summary>
    public void SelectVehicle()
    {
        if (gc.playerObject != null)
            Destroy(gc.playerObject);
        PickVehicle();
    }

    public void SelectVehicle(int id)
    {
        Debug.Log("selected " + id);
        if (id >= vehicles.Length)
            return;
        if (gc.playerObject != null)
            Destroy(gc.playerObject);
        PickVehicle(id);
    }

    public void SelectVehicle(Vehicle vehicle)
    {
        if (gc.playerObject != null)
            Destroy(gc.playerObject);
        PickVehicle(vehicle.id);
    }

    public void PurchaseVehicle(Vehicle vehicle)
    {
        if (vehicle == null || vehicle.upgradeLevels.Length == vehicle.currentLevel + 1 || gc.cash < vehicle.upgradeLevels[vehicle.currentLevel + 1].upgradeCost || gc.cash < vehicle.price)
            return;

        if (!vehicle.purchased)
        {
            // BUY
            gc.cash -= vehicle.price;
            vehicle.purchased = true;
        }
        else if (vehicle.currentLevel + 1 < vehicle.upgradeLevels.Length)
        {
            // UPGRADE
            vehicle.currentLevel++;
            gc.cash -= vehicle.upgradeLevels[vehicle.currentLevel].upgradeCost;
            vehicle.UpdateStats();
            for (int i = 0; i < vehicles.Length; i++)
                vehicles[i].ShowVehicle();
            if (selectedVehicle == vehicle)
                SetAndSpawnVehicle();
        }

        for (int i = 0; i < vehicles.Length; i++)
        {
            vehicles[i].ShowVehicle();
        }
        gc.uc.UpdateCash();
        gc.SaveGame();
    }

    private void PickVehicle()
    {
        selectedVehicle = vehicles[scrollSnap.CurrentPage].vehicle;
        SetAndSpawnVehicle();
    }

    private void PickVehicle(int id)
    {
        selectVehicleButton.GetComponent<Button>().interactable = vehicles[id].vehicle.purchased;
        selectVehicleButton.SetActive(false);
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i].vehicle.id == vehicles[id].vehicle.id)
            {
                selectedVehicle = vehicles[i].vehicle;
                break;
            }
        }
        SetAndSpawnVehicle();
    }

    private void SetAndSpawnVehicle()
    {
        playerObject = selectedVehicle.playerObject;
        gc.pc.SpawnPlayer(playerObject, selectedVehicle);
    }

    private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        try
        {
            selectVehicleButton.GetComponent<Button>().interactable = vehicles[scrollSnap.CurrentPage].vehicle.purchased;
            selectVehicleButton.SetActive(selectedVehicle.id == vehicles[scrollSnap.CurrentPage].vehicle.id ? false : vehicles[scrollSnap.CurrentPage].vehicle.purchased);
        }
        catch (System.NullReferenceException)
        {
            selectVehicleButton.SetActive(false);
        }
    }
}
