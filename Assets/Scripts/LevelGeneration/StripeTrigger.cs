using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripeTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        Stripe parent = GetComponentInParent<Stripe>();
        //if (parent != null && col.gameObject.GetComponentInParent<PlayerUnit>() != null)
        //    parent.ActivateUnits();
    }
}
