using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalCollisionHandler : MonoBehaviour
{
    private PlayerUnit parent;

    private void Awake()
    {
        parent = GetComponentInParent<PlayerUnit>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (parent.currentSpeed <= 0.1f)
            return;
        parent.currentSpeed -= 0.1f;
    }
}
