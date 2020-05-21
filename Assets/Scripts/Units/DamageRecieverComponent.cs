using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRecieverComponent : MonoBehaviour
{
    public int maxHP;
    public bool alwaysReceiveFullDamage;
    public int CurrentHP { get; protected set; }

    public virtual void ReceiveDamage(int damage, Vector3 pos, bool hitByPlayer, bool ShotByPlayer, bool isCritical = false) { }
}
