using Misc;
using System.Collections.Generic;
using UnityEngine;

public class StageStrip : MonoBehaviour
{
    public List<GameObject> allowedObjects;
    public List<GameObject> allowedBoosters;

    public BoxCollider2D personalSpace;
    public BoxCollider2D reachingSpace;

    [Header("Obstacles and Enemies")]
    public int maxObjects;
    public int triesPerObject;

    public List<GameObject> spawnedObjects;
    public List<Enemy> spawnedUnits;

    private float xOffset;
    private float yOffset;

    private ObjectPooler pooler;

    private void Awake()
    {
        pooler = FindObjectOfType<ObjectPooler>();
    }

    void Start()
    {
        spawnedObjects = new List<GameObject>();
        spawnedUnits = new List<Enemy>();
        xOffset = personalSpace.size.x / 2;
        yOffset = personalSpace.size.y / 2;

        SpawnObjects();
    }

    public void SpawnObjects()
    {
        Vector2 pos = gameObject.transform.position;
        GameObject go;
        Obstacle obs = null;

        for (int i = 0; i < maxObjects; i++)
        {
            for (int j = 0; j < triesPerObject; j++)
            {           
                float curX = Random.Range(pos.x - xOffset, pos.x + xOffset);
                float curY = Random.Range(pos.y - yOffset, pos.y + yOffset);

                go = allowedObjects[Random.Range(0, allowedObjects.Count)];
                obs = go.GetComponentInChildren<Obstacle>();

                curY = obs.forceY ? obs.forcedY : curY;

                Vector2 position = new Vector2(curX, curY);
                if (LevelUtils.IsOverlapping(position, obs.personalSpace, spawnedObjects) || !personalSpace.bounds.Contains(position))
                    continue;

                UseObject(go.tag, curX, curY);

                break;
            }
        }

        SpawnBoosters();
    }

    private void SpawnBoosters()
    {
        Vector2 pos = gameObject.transform.position;
        if (allowedBoosters == null)
            return;
        for (int j = 0; j < 100; j++)
        {
            float curX = Random.Range(pos.x - xOffset, pos.x + xOffset);
            float curY = Random.Range(pos.y - yOffset, pos.y + yOffset);

            GameObject go = allowedBoosters[Random.Range(0, allowedBoosters.Count)];
            Obstacle obs = go.GetComponentInChildren<Obstacle>();

            curY = obs.forceY ? obs.forcedY : curY;

            Vector2 position = new Vector2(curX, curY);
            if (LevelUtils.IsOverlapping(position, obs.personalSpace, spawnedObjects) || !personalSpace.bounds.Contains(position))
                continue;

            UseObject(go.tag, curX, curY);

            break;
        }
    }

    private void UseObject(string tag, float x, float y)
    {
        GameObject obj = pooler.GetPooledObject(tag);
        if (obj == null)
        {
            Debug.Log(tag + " is null");
            return;
        }
        obj.transform.parent = transform;
        obj.transform.position = new Vector3(x, y, 0f);
        obj.SetActive(true);
        spawnedObjects.Add(obj);

        Enemy pe;
        if (obj.TryGetComponent(out pe))
            spawnedUnits.Add(pe);
    }

    public void ActivateUnits()
    {
        foreach (Enemy pe in spawnedUnits)
        {
            if(pe.movementController != null)
                pe.movementController.StartMoving();
        }
    }

    public void ClearChildrenObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj == null)
                continue;
            obj.transform.parent = transform.parent.parent;
            obj.SetActive(false);
        }

        spawnedObjects.Clear();
    }
}