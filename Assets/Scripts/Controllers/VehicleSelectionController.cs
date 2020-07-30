using UI;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSelectionController : MonoBehaviour
{
    public VehicleShowcase[] vehicleShowcases;
    public GameObject selectVehicleButton;
    public AudioPlayer successSound;
    public ScrollSnap scrollSnap;
    private GameObject playerObject;

    public Vehicle SelectedVehicle { get; private set; }

    public static VehicleSelectionController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
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

    public void SelectVehicle(int id, int colorId)
    {
        if (id >= vehicleShowcases.Length)
            return;
        if (GameController.Instance.playerObject != null)
            Destroy(GameController.Instance.playerObject);
        PickVehicle(id, colorId);
        vehicleShowcases[scrollSnap.CurrentPage].shadow.SetActive(false);
    }

    public void SelectVehicle(Vehicle vehicle)
    {
        if (GameController.Instance.playerObject != null)
            Destroy(GameController.Instance.playerObject);
        PickVehicle(vehicle.id, vehicle.selectedColorScheme);
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
            Button b = selectVehicleButton.GetComponent<Button>();
            if (b != null) b.interactable = true;
            AchievementController.Instance.UnlockAchievement(GPGSIds.achievement_brand_new);
        }
        else if (vehicle.currentLevel + 1 < vehicle.upgradeLevels.Length)
        {
            // UPGRADE
            vehicle.currentLevel++;
            GameController.Instance.cash -= vehicle.upgradeLevels[vehicle.currentLevel].upgradeCost;
            vehicle.UpdateStats();
            if (SelectedVehicle.id == vehicle.id)
                SetAndSpawnVehicle();
            vehicleShowcases[scrollSnap.CurrentPage].purchaseButton.gameObject.SetActive(vehicle.currentLevel + 1 != vehicle.upgradeLevels.Length);
            vehicleShowcases[scrollSnap.CurrentPage].priceTip.gameObject.SetActive(true);
            AchievementController.Instance.UnlockAchievement(GPGSIds.achievement_level_up);
        }
        successSound?.Play();
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
            successSound?.Play();
            AchievementController.Instance.UnlockAchievement(GPGSIds.achievement_new_look);
        }
        if (SelectedVehicle.id == vehicle.id)
        {
            SelectedVehicle.selectedColorScheme = colorShemeID;
            SetAndSpawnVehicle();
        }

        FinishSelection();
        return true;
    }

    public void UpdateColorPricetags()
    {
        for (int i = 0; i < vehicleShowcases.Length; i++)
        {
            for (int j = 0; j < vehicleShowcases[i].colorShowcases.Length; j++)
            {
                vehicleShowcases[i].colorShowcases[j].UpdatePriceColor();
            }
        }
    }

    private void FinishSelection()
    {
        for (int i = 0; i < vehicleShowcases.Length; i++)
        {
            vehicleShowcases[i].ShowVehicle();
        }
        UIController.Instance.UpdateCash();
        GameController.Instance.SaveGame();
    }

    private void PickVehicle()
    {
        SetupScrollSnap();
        for (int i = 0; i < vehicleShowcases.Length; i++)
        {
            vehicleShowcases[i].colorSchemeScrollView.SetActive(false);
        }
        SelectedVehicle = vehicleShowcases[scrollSnap.CurrentPage].vehicle;
        vehicleShowcases[scrollSnap.CurrentPage].colorSchemeScrollView.SetActive(true);
        vehicleShowcases[scrollSnap.CurrentPage].purchaseButton.gameObject.SetActive(true);
        vehicleShowcases[scrollSnap.CurrentPage].priceTip.gameObject.SetActive(true);
        selectVehicleButton.SetActive(false);
        SetAndSpawnVehicle();
    }

    private void PickVehicle(int id, int colorId)
    {
        SetupScrollSnap();
        for (int i = 0; i < vehicleShowcases.Length; i++)
            vehicleShowcases[i].colorSchemeScrollView.SetActive(false);
        selectVehicleButton.GetComponent<Button>().interactable = vehicleShowcases[id].vehicle.purchased;
        selectVehicleButton.SetActive(false);
        for (int i = 0; i < vehicleShowcases.Length; i++)
        {
            if (vehicleShowcases[i].vehicle.id == vehicleShowcases[id].vehicle.id)
            {
                SelectedVehicle = vehicleShowcases[i].vehicle;
                SelectedVehicle.selectedColorScheme = colorId;
                break;
            }
        }
        vehicleShowcases[scrollSnap.CurrentPage].colorSchemeScrollView.SetActive(true);
        selectVehicleButton.SetActive(false);
        SetAndSpawnVehicle();
    }

    private void SetAndSpawnVehicle()
    {
        if (GameController.Instance.PlayerUnit != null) GameController.Instance.PlayerUnit.Restart();
        playerObject = SelectedVehicle.playerObject;
        LevelController.Instance.SpawnPlayer(playerObject, SelectedVehicle);
        ImageAnimator ia = vehicleShowcases[SelectedVehicle.id].GetComponentInChildren<ImageAnimator>();
        if (ia != null)
        {
            ia.clip = SelectedVehicle.colorSchemes[SelectedVehicle.selectedColorScheme].previewClip;
            ia.sprites = SelectedVehicle.colorSchemes[SelectedVehicle.selectedColorScheme].previewSprites;
        }
        foreach (var vehicleShowcase in vehicleShowcases)
        {
            vehicleShowcase.ShowVehicle();
        }
        GameController.Instance.SaveGame();
    }

    private void OnPageChanged(object sender, PropertyChangedEventArgs e)
    {
        try
        {
            for (int i = 0; i < vehicleShowcases.Length; i++)
                vehicleShowcases[i].shadow.SetActive(true);
            vehicleShowcases[scrollSnap.CurrentPage].shadow?.SetActive(false);
            selectVehicleButton.GetComponent<Button>().interactable = vehicleShowcases[scrollSnap.CurrentPage].vehicle.purchased;
            selectVehicleButton.SetActive(SelectedVehicle.id == vehicleShowcases[scrollSnap.CurrentPage].vehicle.id ? false : vehicleShowcases[scrollSnap.CurrentPage].vehicle.purchased);
            PlaySelectedVehicleAnimation();
        }
        catch (System.NullReferenceException)
        {
            selectVehicleButton.SetActive(false);
        }
    }

    private void PlaySelectedVehicleAnimation()
    {
        for (int i = 0; i < vehicleShowcases.Length; i++)
        {
            vehicleShowcases[i].GetComponentInChildren<ImageAnimator>()?.Stop();
        }
        vehicleShowcases[scrollSnap.CurrentPage].GetComponentInChildren<ImageAnimator>()?.Play();
    }

    private void SetupScrollSnap()
    {
        try
        {
            if (scrollSnap == null || scrollSnap.gameObject.activeSelf) return;
            scrollSnap.gameObject.SetActive(true);
            scrollSnap.PropertyChanged += OnPageChanged;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
}
