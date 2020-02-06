using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainsSpawner : MonoBehaviour
{
    public GameObject[] availableRemains;
    public int remainsAmount;

    private List<GameObject> spawnedRemains;
    private List<GameObject> freeRemains;

    private Vector3 spawnPoint = new Vector3(-200, 0, 0);

    private void Start()
    {
        if (availableRemains == null)
            return;

        spawnedRemains = new List<GameObject>();
        freeRemains = new List<GameObject>();

        for (int i = 0; i < remainsAmount; i++)
        {
            GameObject go = Instantiate(availableRemains[Random.Range(0, availableRemains.Length)], spawnPoint, Quaternion.identity, gameObject.transform);
            spawnedRemains.Add(go);
            freeRemains.Add(go);
            go.SetActive(false);
        }
    }

    public void CreateRemains(Vector3 position, float liveFor)
    {
        GameObject go = freeRemains[Random.Range(0, freeRemains.Count)];
        go.transform.position = position;
        go.SetActive(true);
        StartCoroutine(Recycle(liveFor, go));
    }

    private IEnumerator Recycle(float waitFor, GameObject go)
    {
        yield return new WaitForSeconds(waitFor);
        go.SetActive(false);
        freeRemains.Add(go);
    }

    public void Restart()
    {
        foreach(GameObject go in spawnedRemains)
        {
            go.SetActive(false);
            go.transform.position = spawnPoint;
        }
    }
}
