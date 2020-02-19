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
    private ScrollSnap scrollSnap;

    public Vehicle SelectedVehicle { get; private set; }

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
            if (SelectedVehicle.id == vehicle.id)
                SetAndSpawnVehicle();
        }

        FinishSelection();
    }

    public bool ActivateColorSheme(Vehicle vehicle, int colorShemeID)
    {
        if (vehicle == null || colorShemeID >= vehicle.colorSchemes.Length || gc.cash < vehicle.colorSchemes[colorShemeID].price)
            return false;

        if (!vehicle.colorSchemes[colorShemeID].purchased)
        {
            gc.cash -= vehicle.colorSchemes[colorShemeID].price;

        }
        if (SelectedVehicle.id == vehicle.id)
        {
            SetAndSpawnVehicle();
            SelectedVehicle.selectedColorScheme = colorShemeID;
        }

        FinishSelection();
        return true;
    }

    private void FinishSelection()
    {
        for (int i = 0; i < vehicles.Length; i++)
        {
            vehicles[i].ShowVehicle();
        }
        gc.uc.UpdateCash();
        gc.SaveGame();
    }

    private void PickVehicle()
    {
        for (int i = 0; i < vehicles.Length; i++)
            vehicles[i].colorSchemeScrollView.SetActive(false);
        SelectedVehicle = vehicles[scrollSnap.CurrentPage].vehicle;
        vehicles[scrollSnap.CurrentPage].colorSchemeScrollView.SetActive(true);
        selectVehicleButton.SetActive(false);
        SetAndSpawnVehicle();
    }

    private void PickVehicle(int id)
    {
        for (int i = 0; i < vehicles.Length; i++)
            vehicles[i].colorSchemeScrollView.SetActive(false);
        selectVehicleButton.GetComponent<Button>().interactable = vehicles[id].vehicle.purchased;
        selectVehicleButton.SetActive(false);
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i].vehicle.id == vehicles[id].vehicle.id)
            {
                SelectedVehicle = vehicles[i].vehicle;
                break;
            }
        }
        vehicles[scrollSnap.CurrentPage].colorSchemeScrollView.SetActive(true);
        selectVehicleButton.SetActive(false);
        SetAndSpawnVehicle();
    }

    private void SetAndSpawnVehicle()
    {
        playerObject = SelectedVehicle.playerObject;
        gc.pc.SpawnPlayer(playerObject, SelectedVehicle);
    }

    private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        try
        {
            selectVehicleButton.GetComponent<Button>().interactable = vehicles[scrollSnap.CurrentPage].vehicle.purchased;
            selectVehicleButton.SetActive(SelectedVehicle.id == vehicles[scrollSnap.CurrentPage].vehicle.id ? false : vehicles[scrollSnap.CurrentPage].vehicle.purchased);
        }
        catch (System.NullReferenceException)
        {
            selectVehicleButton.SetActive(false);
        }
    }
}
