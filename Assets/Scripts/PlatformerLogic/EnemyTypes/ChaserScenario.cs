using Assets.Scripts.CustomEventArgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserScenario : EnemyBehaviour
{
    private Coroutine movement;

    public override void HandleCollision(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu == null || UnityEngine.Random.Range(0, 1f) < 0.2f)
            return;

        NextPoint();
        OnEnemyStateChanged(new EnemyStateEventArgs(true));
    }

    protected override void NextPoint()
    {
        if (movement != null) StopCoroutine(movement);

        Vector2 nextPoint = playerTransform.position;

        nextPoint.y = UnityEngine.Random.Range(0, 1f) >= 0.3f ? nextPoint.y : (thisTransform.position.y - nextPoint.y) * UnityEngine.Random.Range(0, 1f);

        if (Vector2.Distance(nextPoint, thisTransform.position) <= distanceThreshold || stop)
            return;

        if (nextPoint.x > thisTransform.position.x)
            thisTransform.localScale = new Vector3(-1, 1, 1);
        else
            thisTransform.localScale = new Vector3(1, 1, 1);

        movement = StartCoroutine(MoveTowards(nextPoint));
    }

    protected override IEnumerator MoveTowards(Vector2 moveTowards)
    {
        while (Vector2.Distance(moveTowards, thisTransform.position) > distanceThreshold)
        {
            Vector3 v = Vector2.MoveTowards(thisTransform.position, moveTowards, speed * Time.deltaTime);
            thisTransform.position = new Vector3(v.x, v.y, 9f + v.y / 10);
            yield return new WaitForEndOfFrame();
        }
        NextPoint();
    }

    protected override IEnumerator BehaviourRoutine()
    {
        while (thisGameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.3f);
            NextPoint();
        }
    }
}
