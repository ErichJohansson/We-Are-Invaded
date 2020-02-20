using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleShowcase : MonoBehaviour
{
    public Vehicle vehicle;
    public Text vehicleName;
    public Image vehicleImage;
    public Text damage;
    public Text health;
    public Text maxSpeed;
    public Text turning;
    public Text reloadTime;
    public Text price;
    public Button selectVehicleButton;
    public GameObject colorSchemeScrollView;

    [Header("Question settings")]
    public GameObject question;
    public Button yesButton;
    public Button noButton;

    public ColorShowcase[] colorShowcases;

    [SerializeField]
    private Sprite lockedVahicle;
    [Header("Price tag colors")]
    [SerializeField]
    private Color notEnoughMoneyColor;
    [SerializeField]
    private Color enoughMoneyColor;

    public void ShowVehicle()
    {
        vehicleName.text = vehicle.name;
        vehicleImage.sprite = vehicle.purchased ? vehicle.colorSchemes[vehicle.selectedColorScheme].previewSprite : lockedVahicle;
        damage.text = vehicle.damage.ToString();
        health.text = vehicle.health.ToString();
        maxSpeed.text = vehicle.speed.ToString();
        turning.text = vehicle.turning.ToString();
        reloadTime.text = vehicle.reloadTime.ToString();

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
