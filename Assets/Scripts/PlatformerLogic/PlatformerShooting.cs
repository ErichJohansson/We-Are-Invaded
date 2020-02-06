using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerShooting : MonoBehaviour, IShotRecycler
{
    public GameObject shot;

    public float cooldownTime;
    public float reloadTime;
    public int magazineCapacity;
    public float shotPower;
    public int shotDamage;

    public Transform firePoint;

    private Animator animator;
    private List<PlatformerShot> spawnedShots;
    private List<PlatformerShot> freeShots;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spawnedShots = new List<PlatformerShot>();
        freeShots = new List<PlatformerShot>();
        for (int i = 0; i < 2 * magazineCapacity; i++)
        {
            GameObject go = Instantiate(shot, firePoint.position, Quaternion.identity, gameObject.transform);
            PlatformerShot platformerShot = go.GetComponent<PlatformerShot>();
            platformerShot.shotRecycler = this;
            platformerShot.damage = shotDamage;
            go.SetActive(false);
            spawnedShots.Add(platformerShot);
            freeShots.Add(platformerShot);
        }

        StartCoroutine("BurstRoutine");
    }

    public void Restart()
    {
        StopAllCoroutines();
        for (int i = 0; i < spawnedShots.Count; i++)
        {
            spawnedShots[i].GetComponent<PlatformerShot>().Recycle(false);
        }
        StartCoroutine("BurstRoutine");
    }

    IEnumerator FireRoutine()
    {
        float t = 0f;
        int i = 0;
        while (i < magazineCapacity)
        {
            if(t >= cooldownTime)
            {
                t = 0f;
                Fire();
                i++;
            }
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    
    IEnumerator BurstRoutine()
    {
        float t = 0f;
        yield return new WaitForSecondsRealtime(Random.Range(0f, 3f));
        while (true)
        {
            if (t >= reloadTime)
            {
                t = -magazineCapacity * cooldownTime;
                StartCoroutine("FireRoutine");
            }
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void Fire()
    {
        animator.SetTrigger("shotStart");        
        freeShots[0].gameObject.SetActive(true);
        freeShots[0].colliding = false;
        freeShots[0].initialY =  new float[] { gameObject.transform.position.y + 0.05f, gameObject.transform.position.y - 0.05f };
        freeShots[0].Activate();
        Rigidbody2D shotRB = freeShots[0].GetComponent<Rigidbody2D>();
        float shotStrength = freeShots[0].baseShotPower * shotPower;
        shotRB.AddForce(new Vector2(shotStrength > 0 ? -shotStrength : shotStrength, 0), ForceMode2D.Impulse);
        freeShots.Remove(freeShots[0]);
    }

    public void RecycleShot(PlatformerShot sender)
    {
        //sender.damageDealt = false;
        freeShots.Add(sender);
        freeShots[freeShots.Count - 1].transform.position = firePoint.position;
    }
}
