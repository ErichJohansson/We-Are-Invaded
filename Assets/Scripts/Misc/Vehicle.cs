using System.Collections;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName ="New Vehicle", menuName = "Create Vehicle")]
public class Vehicle : ScriptableObject
{
    public GameObject playerObject;
    public Sprite previewImage;

    public int id;

    public int health;
    public int armor;
    public float fireRate;
    public float reloadTime;
    public float shotPower;
    public int damage;
    public int magazineCapacity;

    public float speed;
    public float turning;

    public int price;
    public bool purchased;
}
