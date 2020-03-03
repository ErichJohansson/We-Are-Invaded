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


    [Tooltip("10 per level")] public int health;
    public float fireRate;
    [Tooltip("100 / (10 * level) per level")] public float reloadTime;
    [Tooltip("5 per level")] public int damage;
    public int magazineCapacity;
    [Tooltip("1.2 per level")] public float speed;
    [Tooltip("0.2 per level")] public float turning;

    public int price;
    public bool purchased;

    public static int maxDamage = 100;
    public static int maxHealth = 400;
    public static float minReload = 1f;
    public static float maxTurning = 1f;
    public static float maxSpeed = 12f;

    public static int damagePerLevel = 5;
    public static int healthPerLevel = 10;
    public static float reloadPerLevel = 10f;
    public static float turningPerLevel = 0.2f;
    public static float speedPerLevel = 1.2f;

    public UpgradeLevel[] upgradeLevels;

    public void UpdateStats(int level, bool purchased)
    {
        currentLevel = level;
        this.purchased = purchased;
        damage = upgradeLevels[level].damageLevel * damagePerLevel;
        health = upgradeLevels[level].healthLevel * healthPerLevel;
        reloadTime = 200f / (upgradeLevels[level].reloadLevel * reloadPerLevel);
        turning = upgradeLevels[level].turningLevel * turningPerLevel;
        speed = upgradeLevels[level].speedLevel * speedPerLevel;
    }

    public void UpdateStats()
    {
        damage = upgradeLevels[currentLevel].damageLevel * damagePerLevel;
        health = upgradeLevels[currentLevel].healthLevel * healthPerLevel;
        reloadTime = 200f / (upgradeLevels[currentLevel].reloadLevel * reloadPerLevel);
        turning = upgradeLevels[currentLevel].turningLevel * turningPerLevel;
        speed = upgradeLevels[currentLevel].speedLevel * speedPerLevel;
    }

    public static Vehicle GetUpgradedVehicle(Vehicle v)
    {
        if (v.currentLevel + 1 >= v.upgradeLevels.Length)
            return null;

        Vehicle vehi = new Vehicle();
        vehi.damage = v.upgradeLevels[v.currentLevel + 1].damageLevel * damagePerLevel;
        vehi.health = v.upgradeLevels[v.currentLevel + 1].healthLevel * healthPerLevel;
        vehi.reloadTime = 200f / (v.upgradeLevels[v.currentLevel + 1].reloadLevel * reloadPerLevel);
        vehi.turning = v.upgradeLevels[v.currentLevel + 1].turningLevel * turningPerLevel;
        vehi.speed = v.upgradeLevels[v.currentLevel + 1].speedLevel * speedPerLevel;
        return vehi;
    }
}
