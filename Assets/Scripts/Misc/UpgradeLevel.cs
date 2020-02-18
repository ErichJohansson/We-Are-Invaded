using UnityEngine;

[System.Serializable]
public class UpgradeLevel
{
    public int upgradeCost;

    [Tooltip("5 per level")]
    public int damageLevel;
    [Tooltip("10 per level")]
    public int healthLevel;
    [Tooltip("200 / (10 * level) per level")]
    public int reloadLevel;
    [Tooltip("1.2 per level")]
    public int speedLevel;
    [Tooltip("0.1 per level")]
    public int turningLevel;
}
