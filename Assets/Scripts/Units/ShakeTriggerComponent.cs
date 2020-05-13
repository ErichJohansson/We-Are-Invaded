using Assets.Scripts.CustomEventArgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeTriggerComponent : MonoBehaviour
{
    public float length;
    public float amplitude;
    public float frequency;

    private void Awake()
    {
        Enemy e = GetComponent<Enemy>();
        if (e != null) e.DieEvent += OnDeath;

        Obstacle o = GetComponent<Obstacle>();
        if (o != null) o.DieEvent += OnDeath;
    }

    public void OnDeath(object sender, DieEventArgs e)
    {
        CameraShakeController.Instance.ShakeCamera(length, amplitude, frequency);
    }
}
