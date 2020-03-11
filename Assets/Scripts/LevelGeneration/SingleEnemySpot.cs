using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEnemySpot : MonoBehaviour
{
    public List<GameObject> allowedEnemies;

    private List<Enemy> spawnedUnits;
    private ObjectPooler pooler;

    private void Awake()
    {
        spawnedUnits = new List<Enemy>();
        pooler = FindObjectOfType<ObjectPooler>();
    }

    private void UseObject(string tag, float x, float y)
    {
        GameObject obj = pooler.GetPooledObject(tag);
        if (obj == null)
        {
            Debug.Log(tag + " is null");
            return;
        }
        obj.transform.position = new Vector3(x, y, 0f);
        obj.SetActive(true);

        Enemy pe;
        if (obj.TryGetComponent(out pe))
            spawnedUnits.Add(pe);
    }

    private void OnDisable()
    {
        for (int i = 0; i < spawnedUnits.Count; i++)
        {
            if (spawnedUnits[i] != null)
                spawnedUnits[i].gameObject.SetActive(false);
        }
        spawnedUnits.Clear();
    }

    private void OnEnable()
    {
        UseObject(allowedEnemies[Random.Range(0, allowedEnemies.Count)].tag, gameObject.transform.position.x, gameObject.transform.position.y);
    }
}
