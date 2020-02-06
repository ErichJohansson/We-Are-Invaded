using UnityEngine;

[CreateAssetMenu(fileName = "New AttackInstance", menuName = "Create AttackInstance")]
public class AttackInstance : ScriptableObject
{
    public GameObject shot;
    public int totalShots;
    public float force;
    public Vector3 direction;
    public float shotCooldown;

    public int cyclesCount;
    public float cycleCooldown;
}
