using Misc;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    [Header("Spawned objects settings")]

    public Transform spawnPoint;
    public List<GameObject> allowedStrips;

    public int maxStrips;
    public int triesPerStrip;

    public BoxCollider2D chunkArea;

    [Header("Background sprites")]
    public List<Sprite> allowedBackgrounds;
    public SpriteRenderer background;

    private float length;
    private float yOffset;
    private float xOffset;
    private float levelOffest;

    private List<GameObject> spawnedStrips;

    public float Length
    {
        get
        {
            if (length == 0)
                length = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
            return length;
        }
    }

    void Start()
    {
        spawnedStrips = new List<GameObject>();
        xOffset = chunkArea.size.x / 2;
        yOffset = 0;
        levelOffest = Length * 3f;

        SpawnStrips();
    }

    public void SpawnStrips()
    {
        background.sprite = allowedBackgrounds[Random.Range(0, allowedBackgrounds.Count)];
        Vector2 pos = gameObject.transform.position;

        for (int i = 0; i < maxStrips; i++)
        {
            for (int j = 0; j < triesPerStrip; j++)
            {
                float curX = Random.Range(pos.x - xOffset, pos.x + xOffset);
                float curY = Random.Range(pos.y - yOffset, pos.y + yOffset);

                GameObject go = allowedStrips[Random.Range(0, allowedStrips.Count)];
                StageStrip ss = go.GetComponent<StageStrip>();

                Vector2 stripPos = new Vector2(curX, curY);

                if (LevelUtils.IsOverlapping(stripPos, ss.personalSpace, spawnedStrips) || !LevelUtils.IsReaching(stripPos, ss.reachingSpace, spawnedStrips, 1))
                    continue;

                spawnedStrips.Add(Instantiate(go, new Vector3(stripPos.x, stripPos.y, 0f), Quaternion.identity, this.gameObject.transform));
                break;
            }
        }
    }

    public void Regenerate(bool restart = false)
    {
        if (spawnedStrips == null)
            return;

        spawnedStrips.ForEach(x => Destroy(x));
        spawnedStrips.Clear();
        gameObject.transform.position += new Vector3(restart ? 0 : levelOffest, 0, 0);
        SpawnStrips();
    }
}
