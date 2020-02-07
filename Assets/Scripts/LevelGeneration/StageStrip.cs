using Misc;
using System.Collections.Generic;
using UnityEngine;

public class StageStrip : MonoBehaviour
{
    public List<GameObject> allowedObjects;
    public List<GameObject> allowedDecorations;
    public List<GameObject> allowedBoosters;

    public BoxCollider2D personalSpace;
    public BoxCollider2D reachingSpace;

    [Header("Obstacles and Enemies")]
    public int maxObjects;
    public int triesPerObject;
    [Header("Decorations")]
    public int maxDecorObjects;
    public int triesPerDecorObject;

    private float xOffset;
    private float yOffset;

    public List<GameObject> spawnedObjects;
    public List<Enemy> spawnedUnits;

    void Start()
    {
        spawnedObjects = new List<GameObject>();
        spawnedUnits = new List<Enemy>();
        xOffset = personalSpace.size.x / 2;
        yOffset = personalSpace.size.y / 2;

        SpawnObjects();
        SpawnDecorations();
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

                spawnedObjects.Add(Instantiate(go, new Vector3(curX, curY, 0f), Quaternion.identity, this.gameObject.transform));

                Enemy pe;
                if(spawnedObjects[spawnedObjects.Count - 1].TryGetComponent(out pe))
                    spawnedUnits.Add(pe);

                break;
            }
        }

        SpawnBoosters();

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            spawnedObjects[i].GetComponent<Obstacle>().DisableAdditionalColliders();
        }
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

            spawnedObjects.Add(Instantiate(go, new Vector3(curX, curY, 0f), Quaternion.identity, this.gameObject.transform));

            break;
        }
    }

    public void SpawnDecorations()
    {
        Vector2 pos = gameObject.transform.position;
        GameObject go;
        GroundDecor decor = null;

        for (int i = 0; i < maxDecorObjects; i++)
        {
            for (int j = 0; j < triesPerDecorObject; j++)
            {
                float curX = Random.Range(pos.x - xOffset, pos.x + xOffset);
                float curY = Random.Range(pos.y - yOffset, pos.y + yOffset);

                go = allowedDecorations[Random.Range(0, allowedDecorations.Count)];
                decor = go.GetComponentInChildren<GroundDecor>();

                Vector2 position = new Vector2(curX, curY);

                if (LevelUtils.IsOverlapping(position, decor.personalSpace, spawnedObjects) || !personalSpace.bounds.Contains(position))
                    continue;

                spawnedObjects.Add(Instantiate(go, position, Quaternion.identity, this.gameObject.transform));

                Enemy pe;
                if (spawnedObjects[spawnedObjects.Count - 1].TryGetComponent(out pe))
                    spawnedUnits.Add(pe);

                break;
            }
        }
    }

    public void ActivateUnits()
    {
        foreach (Enemy pe in spawnedUnits)
        {
            if(pe.movementController != null)
                pe.movementController.StartMoving();
        }
    }
}