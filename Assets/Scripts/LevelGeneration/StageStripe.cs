using Misc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStripe : MonoBehaviour
{
    public List<GameObject> allowedBoosters;

    public BoxCollider2D personalSpace;
    public BoxCollider2D reachingSpace;

    public List<GameObject> reactivateObjects;

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
    }

    private void OnDisable()
    {
        gameObject.transform.position = new Vector3(-100, 0, 0);
    }

    private void OnEnable()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        if (spawnedObjects.Count != 0)
        {
            //Debug.LogError("More than 0 enemies spawned already");
            ClearChildrenObjects();
        }

        for (int k = 0; k < enemyAreas.Length; k++)
        {
            Vector2 pos = enemyAreas[k].spawnArea.transform.position;
            float xOffset = enemyAreas[k].spawnArea.size.x / 2;
            float yOffset = enemyAreas[k].spawnArea.size.y / 2;
            for (int i = 0; i < enemyAreas[k].maxObjectsInArea; i++)
            {
                for (int j = 0; j < enemyAreas[k].triesPerObject; j++)
                {
                    GameObject go;
                    Obstacle obs = null;

                    float curX = Random.Range(pos.x - xOffset, pos.x + xOffset);
                    float curY = Random.Range(pos.y - yOffset, pos.y + yOffset);

                    go = enemyAreas[k].allowedEnemies[Random.Range(0, enemyAreas[k].allowedEnemies.Count)];
                    obs = go.GetComponentInChildren<Obstacle>();

                    Vector2 position = new Vector2(curX, curY);
                    if (LevelUtils.IsOverlapping(position, obs.personalSpace, spawnedObjects) || !enemyAreas[k].spawnArea.bounds.Contains(position))
                        continue;

                    UseObject(go.tag, curX, curY);
                    break;
                }
            }
        }

        for (int i = 0; i < reactivateObjects.Count; i++)
        {
            reactivateObjects[i].SetActive(true);
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

            Vector2 position = new Vector2(curX, curY);
            if (LevelUtils.IsOverlapping(position, obs.personalSpace) || !personalSpace.bounds.Contains(position))
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

        obj.transform.position = new Vector3(x, y, 0f);
        obj.SetActive(true);

        Enemy pe;
        if (obj.TryGetComponent(out pe))
        {
            spawnedUnits.Add(pe);
            return;
        }

        obj.transform.parent = transform;
        spawnedObjects.Add(obj);
    }

    public void ClearChildrenObjects()
    {
        foreach (Enemy e in spawnedUnits)
        {
            if (e == null || e.IsInActiveState)
                continue;
            e.gameObject.SetActive(false);
        }

        spawnedUnits.Clear();

        foreach (GameObject obj in spawnedObjects)
        {
            if (obj == null)
                continue;
            obj.SetActive(false);
        }

        spawnedObjects.Clear();
    }
}