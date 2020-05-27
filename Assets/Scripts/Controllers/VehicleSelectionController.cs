using System.Collections;
using UI;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSelectionController : MonoBehaviour
{
    public VehicleShowcase[] vehicles;
    public GameObject selectVehicleButton;

    private GameObject playerObject;
    private ScrollSnap scrollSnap;

    public Vehicle SelectedVehicle { get; private set; }

    public static VehicleSelectionController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        scrollSnap = FindObjectOfType<ScrollSnap>();
        scrollSnap.PropertyChanged += PropertyChangedHandler;
    }

    /// <summary>
    /// Call on APPLY button click.
    /// </summary>
    public void SelectVehicle()
    {
        if (GameController.Instance.playerObject != null)
            Destroy(GameController.Instance.playerObject);
        PickVehicle();
    }

    public void SelectVehicle(int id)
    {
        Debug.Log("selected " + id);
        if (id >= vehicles.Length)
            return;
        if (GameController.Instance.playerObject != null)
            Destroy(GameController.Instance.playerObject);
        PickVehicle(id);
    }

    public void SelectVehicle(Vehicle vehicle)
    {
        if (GameController.Instance.playerObject != null)
            Destroy(GameController.Instance.playerObject);
        PickVehicle(vehicle.id);
    }

    public void PurchaseVehicle(Vehicle vehicle)
    {
        if (vehicle == null || vehicle.upgradeLevels.Length == vehicle.currentLevel + 1 || GameController.Instance.cash < vehicle.upgradeLevels[vehicle.currentLevel + 1].upgradeCost || GameController.Instance.cash < vehicle.price)
            return;

        if (!vehicle.purchased)
        {
            // BUY
            GameController.Instance.cash -= vehicle.price;
            vehicle.purchased = true;
            selectVehicleButton.SetActive(true);
            selectVehicleButton.GetComponent<Button>().interactable = vehicle.purchased;
        }
        else if (vehicle.currentLevel + 1 < vehicle.upgradeLevels.Length)
        {
            // UPGRADE
            vehicle.currentLevel++;
            GameController.Instance.cash -= vehicle.upgradeLevels[vehicle.currentLevel].upgradeCost;
            vehicle.UpdateStats();
            if (SelectedVehicle.id == vehicle.id)
                SetAndSpawnVehicle();
        }

        FinishSelection();
    }

    public bool RaisePurchaseVehicleQuestion(Vehicle vehicle)
    {
        if (vehicle == null || vehicle.upgradeLevels.Length == vehicle.currentLevel + 1 || GameController.Instance.cash < vehicle.upgradeLevels[vehicle.currentLevel + 1].upgradeCost || GameController.Instance.cash < vehicle.price)
            return false;
        return true;
    }
    
    public bool RaiseActivateColorShemeQuestion(Vehicle vehicle, int colorShemeID) {
        if (vehicle == null || colorShemeID >= vehicle.colorSchemes.Length || GameController.Instance.cash < vehicle.colorSchemes[colorShemeID].price)
            return false;
        return true;
    }

    public bool ActivateColorSheme(Vehicle vehicle, int colorShemeID)
    {
        if (colorShemeID >= vehicle.colorSchemes.Length || !vehicle.colorSchemes[colorShemeID].purchased && (vehicle == null || GameController.Instance.cash < vehicle.colorSchemes[colorShemeID].price))
            return false;

        if (!vehicle.colorSchemes[colorShemeID].purchased)
        {
            GameController.Instance.cash -= vehicle.colorSchemes[colorShemeID].price;
            vehicle.colorSchemes[colorShemeID].purchased = true;
        }
        if (SelectedVehicle.id == vehicle.id)
        {
            SelectedVehicle.selectedColorScheme = colorShemeID;
            SetAndSpawnVehicle();
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
        UIController.Instance.UpdateCash();
        GameController.Instance.SaveGame();
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
        if (GameController.Instance.PlayerUnit != null) GameController.Instance.PlayerUnit.Restart();
        playerObject = SelectedVehicle.playerObject;
        LevelController.Instance.SpawnPlayer(playerObject, SelectedVehicle);
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
