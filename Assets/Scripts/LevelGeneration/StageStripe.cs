using Misc;
using System.Collections.Generic;
using UnityEngine;

public class StageStripe : MonoBehaviour
{
    public List<GameObject> allowedBoosters;

    public BoxCollider2D personalSpace;
    public BoxCollider2D reachingSpace;

    [Header("Enemies")]
    public EnemyArea[] enemyAreas;

    [HideInInspector] public List<GameObject> spawnedObjects;
    [HideInInspector] public List<Enemy> spawnedUnits;

    private ObjectPooler pooler;

    private void Awake()
    {
        pooler = FindObjectOfType<ObjectPooler>();
    }

    void Start()
    {
        spawnedObjects = new List<GameObject>();
        spawnedUnits = new List<Enemy>();

        SpawnObjects();
    }

    public void SpawnObjects()
    {
        GameObject go;
        Obstacle obs = null;

        for (int k = 0; k < enemyAreas.Length; k++)
        {
            Vector2 pos = enemyAreas[k].spawnArea.transform.position;
            float xOffset = enemyAreas[k].spawnArea.size.x / 2;
            float yOffset = enemyAreas[k].spawnArea.size.y / 2;
            for (int i = 0; i < enemyAreas[k].maxObjectsInArea; i++)
            {
                for (int j = 0; j < enemyAreas[k].triesPerObject; j++)
                {
                    float curX = Random.Range(pos.x - xOffset, pos.x + xOffset);
                    float curY = Random.Range(pos.y - yOffset, pos.y + yOffset);

                    go = enemyAreas[k].allowedEnemies[Random.Range(0, enemyAreas[k].allowedEnemies.Count)];
                    obs = go.GetComponentInChildren<Obstacle>();

                    curY = obs.forceY ? obs.forcedY : curY;

                    Vector2 position = new Vector2(curX, curY);
                    if (LevelUtils.IsOverlapping(position, obs.personalSpace, spawnedObjects) || !personalSpace.bounds.Contains(position))
                        continue;

                    UseObject(go.tag, curX, curY);

                    break;
                }
            }
        }

        SpawnBoosters();
    }

    private void SpawnBoosters()
    {
        Vector2 pos = gameObject.transform.position;
        if (allowedBoosters == null)
            return;
        float xOffset = personalSpace.size.x / 2;
        float yOffset = personalSpace.size.y / 2;
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
            obj.SetActive(false);
        }

        spawnedObjects.Clear();
        spawnedUnits.Clear();
    }
}