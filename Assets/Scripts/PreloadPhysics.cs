using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PreloadPhysics : MonoBehaviour
{
    public UnityEvent unityEvent;

    private void Awake()
    {
        //Physics2D.Simulate(.001f);
    }
}
