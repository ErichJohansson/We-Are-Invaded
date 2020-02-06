using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        StageStrip parent = GetComponentInParent<StageStrip>();
        if (parent != null && col.gameObject.GetComponentInParent<PlayerUnit>() != null)
            parent.ActivateUnits();
    }
}
