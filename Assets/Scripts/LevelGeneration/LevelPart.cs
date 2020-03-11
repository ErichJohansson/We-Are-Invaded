using Misc;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    [Header("Spawned objects settings")]
    public Transform spawnPoint;
    public List<GameObject> allowedStripes;

    public int maxStrips;
    public int triesPerStrip;

    public BoxCollider2D chunkArea;

    public int ID;

    [Header("Background sprites")]
    public List<Sprite> allowedBackgrounds;
    public SpriteRenderer background;

    private float length;
    private float yOffset;
    private float xOffset;
    private float levelOffest;

    private List<GameObject> spawnedStripes;
    private ObjectPooler pooler;

    public float Length
    {
        get
        {
            if (length == 0)
                length = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
            return length;
        }
    }

    private void Awake()
    {
        pooler = FindObjectOfType<ObjectPooler>();
        spawnedStripes = new List<GameObject>();
    }

    void Start()
    {
        xOffset = chunkArea.size.x / 2;
        yOffset = 0;
        levelOffest = Length * 3f;

        SpawnStripes();
    }

    private void OnDisable()
    {
        foreach (GameObject stripe in spawnedStripes)
        {
            stripe.GetComponent<StageStripe>().ClearChildrenObjects();
            stripe.SetActive(false);
        }
    }

    public void SpawnStripes()
    {
        background.sprite = allowedBackgrounds[Random.Range(0, allowedBackgrounds.Count)];
        Vector2 pos = gameObject.transform.position;

        for (int i = 0; i < maxStrips; i++)
        {
            for (int j = 0; j < triesPerStrip; j++)
            {
                float curX = Random.Range(chunkArea.offset.x + pos.x - xOffset, chunkArea.offset.x + pos.x + xOffset);
                float curY = Random.Range(chunkArea.offset.y + pos.y - yOffset, chunkArea.offset.y + pos.y + yOffset);

                GameObject go = allowedStripes[Random.Range(0, allowedStripes.Count)];
                StageStripe ss = go.GetComponent<StageStripe>();

                Vector2 stripePos = new Vector2(curX, curY);

                if (LevelUtils.IsOverlapping(stripePos, ss.personalSpace, spawnedStripes) || !LevelUtils.IsReaching(stripePos, ss.reachingSpace, spawnedStripes, 1))
                    continue;

                UseObject(go.tag, stripePos.x, stripePos.y);

                break;
            }
        }
    }

    public void Regenerate(bool restart = false)
    {
        if (spawnedStripes == null)
            return;

        foreach(GameObject stripe in spawnedStripes)
        {
            stripe.GetComponent<StageStripe>().ClearChildrenObjects();
            stripe.SetActive(false);
        }
        spawnedStripes.Clear();

        gameObject.transform.position += new Vector3(restart ? 0 : levelOffest, 0, 0);

        LevelPart lp = FindObjectOfType<PlatformerController>().GetSpawnedPart(ID == 0 ? 2 : ID - 1).GetComponent<LevelPart>();
        if (lp.transform.position.x - gameObject.transform.position.x < -384)
            lp.Regenerate();

        SpawnStripes();
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
        spawnedStripes.Add(obj);
    }
}
