using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectSpawner : MonoBehaviour
{
    public GameObject[] availableEffects;
    public int effectsAmount;

    private List<GameObject> spawnedEffects;
    private List<GameObject> freeEffects;

    private Vector3 spawnPoint = new Vector3(-200, 0, 0);

    private void Start()
    {
        if (availableEffects == null)
            return;

        spawnedEffects = new List<GameObject>();
        freeEffects = new List<GameObject>();

        for (int i = 0; i < effectsAmount; i++)
        {
            GameObject go = Instantiate(availableEffects[Random.Range(0, availableEffects.Length)], spawnPoint, Quaternion.identity, gameObject.transform);
            spawnedEffects.Add(go);
            freeEffects.Add(go);
            go.SetActive(false);
        }
    }

    public void CreateEffect(Vector3 position, float liveFor)
    {
        GameObject go = freeEffects[Random.Range(0, freeEffects.Count)];
        go.transform.position = position;
        go.SetActive(true);
        StartCoroutine(Recycle(liveFor, go));
    }

    private IEnumerator Recycle(float waitFor, GameObject go)
    {
        yield return new WaitForSeconds(waitFor);
        go.SetActive(false);
        freeEffects.Add(go);
    }
}