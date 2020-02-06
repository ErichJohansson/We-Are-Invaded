using Misc;
using System.Collections.Generic;
using UnityEngine;

public class StageStrip : MonoBehaviour
{
    public List<GameObject> allowedObjects;
    public List<GameObject> allowedDecorations;

    public BoxCollider2D personalSpace;
    public BoxCollider2D reachingSpace;

    [Header("Obstacles and Enemies")]
    public int maxObjects;
    public int triesPerObject;
    [Header("Decorations")]
    public int maxDecorObjects;
    public int triesPerDecorObject;

    public GameObject pickable;
    [Header("Min and Max number of rows with pickable objects")]
    public int minPickableRows;
    public int maxPickableRows;
    [Header("Min and Max number of pickables in a row")]
    public int minPickablesInRow;
    public int maxPickablesInRow;

    private float xOffset;
    private float yOffset;

    //private bool createPickableRow;

    private List<GameObject> spawnedObjects;
    private List<GameObject> spawnedPickables;
    public List<Enemy> spawnedUnits;

    void Start()
    {
        spawnedObjects = new List<GameObject>();
        spawnedPickables = new List<GameObject>();
        spawnedUnits = new List<Enemy>();
        xOffset = personalSpace.size.x / 2;
        yOffset = personalSpace.size.y / 2;

        SpawnObjects();
        SpawnDecorations();
    }

    public void SpawnObjects()
    {
        Vector2 pos = gameObject.transform.position;
        //Collider2D[] overlapping = new Collider2D[] { };
        int spawnedPickableRows = 0;
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

        go = pickable;
        obs = go.GetComponent<Obstacle>();
        int willBeSpawned = Random.Range(minPickableRows, maxPickableRows);
        while (spawnedPickableRows <= willBeSpawned)
        {
            float curX = Random.Range(pos.x - xOffset, pos.x + xOffset);
            float curY = Random.Range(pos.y - yOffset, pos.y + yOffset);        

            if (LevelUtils.IsOverlapping(new Vector2(curX, curY), obs.personalSpace, spawnedObjects))
                continue;

            spawnedPickableRows++;
            CreatePickableRow(pickable, new Vector2(curX, curY));
        }

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            spawnedObjects[i].GetComponent<Obstacle>().DisableAdditionalColliders();
        }
    }

    public void SpawnDecorations()
    {
        Vector2 pos = gameObject.transform.position;
        Collider2D[] overlapping = new Collider2D[] { };
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

    private void CreatePickableRow(GameObject pickable, Vector2 randomPos)
    {
        Vector2 prevPos = randomPos;

        GameObject go = pickable;
        Obstacle obs = go.GetComponent<Obstacle>();

        float xOffsetPickable = obs.reachingSpace.size.x;
        float yOffsetPickable = obs.reachingSpace.size.y / 2;

        int pickablesCount = Random.Range(minPickablesInRow, maxPickablesInRow);
        //Debug.Log(pickablesCount.ToString() + " in a row");
        for (int i = 0; i < pickablesCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                float curX = Random.Range(prevPos.x, prevPos.x + xOffsetPickable);
                float curY = Random.Range(prevPos.y - yOffsetPickable, prevPos.y + yOffsetPickable);

                Vector2 pickablePos = new Vector2(curX, curY);

                if (LevelUtils.IsOverlapping(pickablePos, obs.personalSpace, spawnedObjects) || LevelUtils.IsOverlapping(pickablePos, obs.personalSpace, spawnedPickables)
                    || !LevelUtils.IsReaching(pickablePos, obs.reachingSpace, spawnedPickables, 1) || !personalSpace.bounds.Contains(pickablePos))
                    continue;

                prevPos = pickablePos;
                spawnedPickables.Add(Instantiate(go, new Vector3(curX, curY, 0f), Quaternion.identity, this.gameObject.transform));
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