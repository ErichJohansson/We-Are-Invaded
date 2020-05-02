﻿using System.Collections.Generic;
using UnityEngine;

public class RemainsComponent : MonoBehaviour
{
    public string[] allowedRemainsTags;
    public Transform bloodSplatterPos;
    private ObjectPooler pooler;
    private DeactivateComponent deactivateComponent;

    private void Awake()
    {
        Enemy e = GetComponent<Enemy>();
        if (e != null) e.DieEvent += OnDeath;

        pooler = FindObjectOfType<ObjectPooler>();
        deactivateComponent = GetComponent<DeactivateComponent>();
    }

    public void OnDeath(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("remains");
        CreateRemains();
    }

    private void CreateRemains()
    {
        if (allowedRemainsTags.Length != 0)
        {
            GameObject obj = pooler.GetPooledObject(allowedRemainsTags[Random.Range(0, allowedRemainsTags.Length)]);
            obj.transform.position = new Vector3(bloodSplatterPos.position.x, bloodSplatterPos.position.y, 9 + bloodSplatterPos.position.y / 10f);
            obj.SetActive(true);
            if(deactivateComponent != null) StartCoroutine(deactivateComponent.DeactivateAfterDelay(15f, obj));
        }
    }
}