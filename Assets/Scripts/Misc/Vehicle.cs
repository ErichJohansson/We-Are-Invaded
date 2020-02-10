using System.Collections;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName ="New Vehicle", menuName = "Create Vehicle")]
public class Vehicle : ScriptableObject
{
    public GameObject playerObject;
    public Sprite[] colorSchemes;
    public RuntimeAnimatorController[] animatorControllers;

    public int id;
    public int selectedColorScheme;

    public int health;
    public float fireRate;
    public float reloadTime;
    public int damage;
    public int magazineCapacity;

    public float speed;
    public float turning;

    public int price;
    public bool purchased;

    public TankData defaultStats;
}
