using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private GameController gc;

    private void Awake()
    {
        gc = FindObjectOfType<GameController>();
        priceTag.text = showcase.vehicle.colorSchemes[id].price.ToString();
    }

    public void ShowScheme()
    {
        priceTag.color = showcase.vehicle.colorSchemes[id].price < gc.cash ? enoughMoneyColor : notEnoughMoneyColor;
        priceTag.gameObject.SetActive(!showcase.vehicle.colorSchemes[id].purchased);
    }

    public void BuyOrSelectColor()
    {
        if (gc.vehicleSelection.ActivateColorSheme(showcase.vehicle, id))
        {
            priceTag.gameObject.SetActive(false);
        }
    }
}
