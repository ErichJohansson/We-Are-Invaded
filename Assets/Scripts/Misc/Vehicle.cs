using System.Collections;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName ="New Vehicle", menuName = "Create Vehicle")]
public class Vehicle : ScriptableObject
{
    public GameObject playerObject;
    public ColorScheme[] colorSchemes;

    public int id;
    public int selectedColorScheme;
    public int currentLevel;
    public string name;

    [Tooltip("10 per level")]
    public int health;
    public float fireRate;
    [Tooltip("100 / (10 * level) per level")]
    public float reloadTime;
    [Tooltip("5 per level")]
    public int damage;
    public int magazineCapacity;

    [Tooltip("1.92 per level")]
    public float speed;
    [Tooltip("0.1 per level")]
    public float turning;

    public int price;
    public bool purchased;

    public UpgradeLevel[] upgradeLevels;

    public void UpdateStats(int level, bool purchased)
    {
        currentLevel = level;
        this.purchased = purchased;
        damage = upgradeLevels[level].damageLevel * 5;
        health = upgradeLevels[level].healthLevel * 10;
        reloadTime = 200f / (upgradeLevels[level].reloadLevel * 10f);
        turning = upgradeLevels[level].turningLevel * 0.1f;
        speed = upgradeLevels[level].speedLevel * 1.2f;
    }

    public void UpdateStats()
    {
        damage = upgradeLevels[currentLevel].damageLevel * 5;
        health = upgradeLevels[currentLevel].healthLevel * 10;
        reloadTime = 200f / (upgradeLevels[currentLevel].reloadLevel * 10f);
        turning = upgradeLevels[currentLevel].turningLevel * 0.1f;
        speed = upgradeLevels[currentLevel].speedLevel * 1.2f;
    }
}
