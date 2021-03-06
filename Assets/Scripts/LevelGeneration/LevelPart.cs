﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    [Header("Spawned objects settings")]
    public Transform fillerTransform;
    public Transform postFillerTransform;
    public Transform spawnPoint;
    public List<GameObject> allowedStripes;

    public int maxStripes;
    public int minStripes;
    public int triesPerStrip;

    public BoxCollider2D chunkArea;

    public int ID;

    [Header("Background sprites")]
    public List<LevelAppeareance> backgrounds;
    public SpriteRenderer background;
    public SpriteRenderer foreground;

    private float length;
    private float yOffset;
    private float xOffset;
    private float levelOffest;

    [SerializeField ]private List<GameObject> spawnedStripes;
    private ObjectPooler pooler;
    private GameObject filler;
    private GameObject postFiller;
    Transform thisTransform;

    public float Length
    {
        get
        {
            if (length == 0)
                length = GetComponentsInChildren<SpriteRenderer>().Where(x => x.sprite != null).ToArray()[0].bounds.size.x;
            return length;
        }
    }

    private void Awake()
    {
        thisTransform = gameObject.transform;
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
            stripe.GetComponent<Stripe>().ClearChildrenObjects();
            stripe.SetActive(false);
        }
    }

    private IEnumerator SpawnStripes()
    {
        LevelAppeareance la = backgrounds[Random.Range(0, backgrounds.Count)];
        background.sprite = la.background;
        background.sortingOrder = la.bgrLayer;
        background.transform.localPosition = new Vector3(la.bgrOffset.x, la.bgrOffset.y);

        foreground.sprite = la.foreground;
        foreground.color = new Color(foreground.color.r, foreground.color.g, foreground.color.b, la.fgrOpacity);
        foreground.sortingOrder = la.fgrLayer;
        foreground.transform.localPosition = new Vector3(la.fgrOffset.x, foreground.transform.localPosition.y);


        if (filler != null) Destroy(filler);
        if (la.filler != null)
        {
            GameObject filler = Instantiate(la.filler, fillerTransform.position, Quaternion.identity, thisTransform);
            this.filler = filler;
        }
        if (postFiller != null) Destroy(postFiller);
        if (la.postFiller != null)
        {
            GameObject filler = Instantiate(la.postFiller, postFillerTransform.position, Quaternion.identity, thisTransform);
            this.postFiller = filler;
        }


        Vector2 pos = thisTransform.position;

        float prevX = spawnPoint.position.x + 10f;
        float endX = thisTransform.position.x + chunkArea.size.x / 2;

        while (prevX + 5 <= endX)
        {
            yield return new WaitForEndOfFrame();
            GameObject go = allowedStripes[Random.Range(0, allowedStripes.Count)];
            Stripe ss = go.GetComponent<Stripe>();

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
            stripe.GetComponent<Stripe>().ClearChildrenObjects();
            stripe.SetActive(false);
        }
        spawnedStripes.Clear();

        gameObject.transform.position += new Vector3(restart ? 0 : levelOffest, 0, 0);

        LevelPart lp = FindObjectOfType<LevelController>().GetSpawnedPart(ID == 0 ? 2 : ID - 1).GetComponent<LevelPart>();
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
