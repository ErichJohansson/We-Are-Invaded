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


    public int health;
    public float fireRate;
    public float reloadTime;
    public int damage;
    public int magazineCapacity;
    public float speed;
    public float turning;

    public int price;
    public bool purchased;

    public static int maxDamage = 50;
    public static int maxHealth = 100;
    public static float minReload = 1f;
    public static float maxTurning = 1f;
    public static float maxSpeed = 15f;

    public UpgradeLevel[] upgradeLevels;

    public void UpdateStats(int level, bool purchased)
    {
        currentLevel = level;
        this.purchased = purchased;
        damage = upgradeLevels[level].damage;
        health = upgradeLevels[level].health;
        reloadTime = upgradeLevels[level].reload;
        turning = upgradeLevels[level].turning;
        speed = upgradeLevels[level].speed;
    }

    public void UpdateStats()
    {
        damage = upgradeLevels[currentLevel].damage;
        health = upgradeLevels[currentLevel].health;
        reloadTime = upgradeLevels[currentLevel].reload;
        turning = upgradeLevels[currentLevel].turning;
        speed = upgradeLevels[currentLevel].speed;
    }

    public static Vehicle GetUpgradedVehicle(Vehicle v)
    {
        if (v.currentLevel + 1 >= v.upgradeLevels.Length)
            return null;

        Vehicle vehi = new Vehicle();
        vehi.purchased = v.purchased;
        vehi.damage = v.upgradeLevels[v.currentLevel + 1].damage;
        vehi.health = v.upgradeLevels[v.currentLevel + 1].health;
        vehi.reloadTime = v.upgradeLevels[v.currentLevel + 1].reload;
        vehi.turning = v.upgradeLevels[v.currentLevel + 1].turning;
        vehi.speed = v.upgradeLevels[v.currentLevel + 1].speed;
        return vehi;
    }
}
