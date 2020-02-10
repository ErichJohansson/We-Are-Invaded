using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public bool autoStart;
    [Header("Shooting")]
    public BoxCollider2D affectedArea;
    public Transform startPoint;
    public Transform endPoint;
    public int damage;
    public float moveTime;
    public float reloadTime;
    public float hitTick;
    public bool noReturn;

    private bool reloading;
    private Transform fromP;
    private Transform toP;

    [Header("Effects")]
    public GameObject groundHitAnimator;
    public Animator unitAnimator;

    private void OnEnable()
    {
        reloading = false;
        affectedArea.enabled = false;
        fromP = startPoint;
        toP = endPoint;
        if (autoStart)
            StartCoroutine("StartRoutine");
    }

    private void ApplyDamage(Collider2D col, int damage)
    {
        Debug.Log(col.gameObject.layer);
        PlayerUnit pu;
        if (col.transform.parent.TryGetComponent(out pu))
        {
            pu.DealDamage(damage, pu.transform.position, damage > this.damage);
        }
    }

    public void Stop()
    {
        StopAllCoroutines();
        reloading = true;
        groundHitAnimator.SetActive(false);
    }

    #region Coroutines
    private IEnumerator StartRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0, 2f));
        StartCoroutine(Firing(fromP, toP));
    }

    private IEnumerator Reload()
    {
        groundHitAnimator.SetActive(false);
        unitAnimator.SetTrigger("idle");
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
        StartCoroutine(Firing(fromP, toP));
    }

    private IEnumerator Firing(Transform from, Transform to)
    {
        float t = 0;
        fromP = noReturn ? from : to;
        toP = noReturn ? to : from;
        StartCoroutine("HitTick");
        unitAnimator.SetTrigger("shot");
        groundHitAnimator.SetActive(true);
        while(t < moveTime && !reloading)
        {
            t += Time.deltaTime;
            groundHitAnimator.transform.position = Vector3.Lerp(fromP.position, toP.position, t / moveTime);
            yield return new WaitForEndOfFrame();
        }
        StopCoroutine("HitTick");
        StartCoroutine("Reload");
    }

    private IEnumerator HitTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(hitTick);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(groundHitAnimator.transform.position, affectedArea.size, 0, 1 << 16);
            if (colliders.Length == 0)
            {
                //Debug.Log("col = null");
                continue;
            }
            new List<Collider2D>(colliders).ForEach(col => ApplyDamage(col, (int)Random.Range(damage - 0.25f * damage, damage + 0.15f * damage)));
        }
    }
    #endregion
}