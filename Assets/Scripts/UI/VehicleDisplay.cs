using UnityEngine.UI;
using UnityEngine;

public class VehicleDisplay : MonoBehaviour
{
    public Vehicle vehicle;

    private void Start()
    {
        GetComponent<Image>().sprite = vehicle.previewImage;
    }
}
