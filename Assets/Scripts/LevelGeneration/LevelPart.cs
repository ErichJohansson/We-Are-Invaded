using Misc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    [Header("Spawned objects settings")]
    public Transform spawnPoint;
    public List<GameObject> allowedStripes;

    public int maxStripes;
    public int minStripes;
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

    [SerializeField ]private List<GameObject> spawnedStripes;
    private ObjectPooler pooler;
    Transform trnsfrm;

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
        trnsfrm = gameObject.transform;
        pooler = FindObjectOfType<ObjectPooler>();
        spawnedStripes = new List<GameObject>();
    }

    void Start()
    {
        xOffset = chunkArea.size.x / 2;
        yOffset = 0;
        levelOffest = Length * 3f;

        StartCoroutine(SpawnStripes());
    }

    private void OnDisable()
    {
        foreach (GameObject stripe in spawnedStripes)
        {
            stripe.GetComponent<StageStripe>().ClearChildrenObjects();
            stripe.SetActive(false);
        }
    }

    private IEnumerator SpawnStripes()
    {
        background.sprite = allowedBackgrounds[Random.Range(0, allowedBackgrounds.Count)];
        Vector2 pos = trnsfrm.position;

        float prevX = spawnPoint.position.x + 10f;
        float endX = trnsfrm.position.x + chunkArea.size.x / 2;

        while (prevX + 5 <= endX)
        {
            yield return new WaitForEndOfFrame();
            GameObject go = allowedStripes[Random.Range(0, allowedStripes.Count)];
            StageStripe ss = go.GetComponent<StageStripe>();

            float curX = prevX + ss.personalSpace.size.x + 1f;
            float curY = Random.Range(chunkArea.offset.y + pos.y - yOffset, chunkArea.offset.y + pos.y + yOffset);
            Vector2 stripePos = new Vector2(curX, curY);

            UseObject(go.tag, stripePos.x, stripePos.y);
            prevX = curX;
        }
    }


    public void Regenerate(bool restart = false)
    {
        if (spawnedStripes == null)
            return;
        Debug.Log("regenerated lp");
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

        StartCoroutine(SpawnStripes());
    }

    private void UseObject(string tag, float x, float y)
    {
        GameObject obj = pooler.GetPooledObject(tag);
        if (obj == null)
        {
            Debug.Log(tag + " is null");
            return;
        }
        spawnedStripes.Add(obj);
        obj.transform.parent = transform;
        obj.transform.position = new Vector3(x, y, 0f);
        obj.SetActive(true);
    }
}
