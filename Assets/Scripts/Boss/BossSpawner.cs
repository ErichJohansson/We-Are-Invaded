using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public float mtth;
    public float tickTime;
    public List<GameObject> allBosses;
    private bool bossSpawned;

    public GameObject currentBoss;

    public static BossSpawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine("SpawnBoss");
    }

    private IEnumerator SpawnBoss()
    {
        float t = 0;
        while (!bossSpawned)
        {
            t += tickTime;
            if (Random.Range(0, 1f) < t / mtth)
            {
                //Enemy.SetActiveEnemies(false);

                currentBoss = Instantiate(allBosses[Random.Range(0, allBosses.Count)], new Vector3(GameController.Instance.PlayerUnit.transform.position.x + 15f, 0, 9),
                    Quaternion.identity, GameController.Instance.PlayerUnit.transform.parent);
                bossSpawned = true;
            }
            yield return new WaitForSeconds(tickTime);
        }
    }

    public void Restart()
    {
        StopAllCoroutines();
        if (currentBoss != null)
            Destroy(currentBoss);
        if(bossSpawned)
            bossSpawned = false;
        StartCoroutine("SpawnBoss");
    }
}
