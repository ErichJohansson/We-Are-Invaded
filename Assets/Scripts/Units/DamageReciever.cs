using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReciever : MonoBehaviour
{
    public int maxHP;

    public int CurrentHP { get; protected set; }

    public virtual void ReceiveDamage(int damage, Vector3 pos, bool hitByPlayer, bool isCritical = false) { }
}
