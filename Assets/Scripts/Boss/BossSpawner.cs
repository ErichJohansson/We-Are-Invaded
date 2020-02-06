using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public float mtth;
    public float tickTime;
    public List<GameObject> allBosses;
    private GameController gc;
    private bool bossSpawned;

    public GameObject currentBoss;

    private void Start()
    {
        gc = FindObjectOfType<GameController>();
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
                Enemy.SetActiveEnemies(false);

                currentBoss = Instantiate(allBosses[Random.Range(0, allBosses.Count)], new Vector3(gc.PlayerUnit.transform.position.x + 15f, 0, 9),
                    Quaternion.identity, gc.PlayerUnit.transform.parent);
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
