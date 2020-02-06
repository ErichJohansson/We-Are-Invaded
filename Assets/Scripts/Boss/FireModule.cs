using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FireModule : MonoBehaviour, IShotRecycler
{
    public List<AttackInstance> attackInstances;
    private int instanceId;
    public float attackInstanceDelay;

    private AttackInstance currentAttackInstance;
    private List<PlatformerShot> freeShots;
    private List<PlatformerShot> spawnedShots;

    private void Start()
    {
        instanceId = 0;
        currentAttackInstance = attackInstances[instanceId];

        spawnedShots = new List<PlatformerShot>();
        freeShots = new List<PlatformerShot>();

        for (int i = 0; i < 2 * currentAttackInstance.totalShots; i++)
        {
            spawnedShots.Add(Instantiate(currentAttackInstance.shot, gameObject.transform.position, Quaternion.identity, gameObject.transform.parent.parent).GetComponent<PlatformerShot>());
            spawnedShots[spawnedShots.Count - 1].playerControlled = true;
            spawnedShots[spawnedShots.Count - 1].shotRecycler = this;
            freeShots.Add(spawnedShots[spawnedShots.Count - 1]);
            spawnedShots[spawnedShots.Count - 1].playerControlled = this;
            spawnedShots[spawnedShots.Count - 1].damage = spawnedShots[spawnedShots.Count - 1].damage;
            spawnedShots[spawnedShots.Count - 1].gameObject.SetActive(false);
        }

        StartCoroutine("FireRoutine");
    }

    IEnumerator FireRoutine()
    {
        float t = 0f;
        int shotsFired = 0;
        int cyclesPassed = 0;
        while(instanceId < attackInstances.Count)
        {
            while (cyclesPassed < currentAttackInstance.cyclesCount)
            {
                while (shotsFired < currentAttackInstance.totalShots)
                {
                    if (t >= currentAttackInstance.shotCooldown)
                    {
                        t = 0f;
                        Fire();
                        shotsFired++;
                    }
                    t += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                cyclesPassed++;
                shotsFired = 0;
                yield return new WaitForSeconds(currentAttackInstance.cycleCooldown);
            }
            instanceId = 0;
            currentAttackInstance = attackInstances[instanceId];
            shotsFired = 0;
            cyclesPassed = 0;
            t = 0f;
        }
    }

    private void Fire()
    {
        //animator.SetTrigger("shotStart");
        freeShots[0].transform.position = gameObject.transform.position;
        freeShots[0].gameObject.SetActive(true);
        freeShots[0].colliding = false;
        freeShots[0].Activate();
        Rigidbody2D shotRB = freeShots[0].GetComponent<Rigidbody2D>();
        float shotStrength = freeShots[0].baseShotPower * currentAttackInstance.force;
        shotRB.AddForce(new Vector2(currentAttackInstance.direction.x * shotStrength, currentAttackInstance.direction.y), ForceMode2D.Impulse);
        freeShots.Remove(freeShots[0]);
    }

    public void RecycleShot(PlatformerShot sender)
    {
        //sender.damageDealt = false;
        if (this == null)
            return;
        freeShots.Add(sender);
        freeShots[freeShots.Count - 1].transform.position = gameObject.transform.position;
    }
}
