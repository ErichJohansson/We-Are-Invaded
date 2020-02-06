using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    GameController gc;

    void Awake()
    {
        gc = FindObjectOfType<GameController>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.transform.parent.gameObject == gc.playerObject)
        {
            gc.uc.AddScore(1);
        }
    }
}
