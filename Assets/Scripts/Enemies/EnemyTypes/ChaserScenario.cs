﻿using Assets.Scripts.CustomEventArgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserScenario : EnemyBehaviour
{
    public AudioPlayer triggerSound;

    private Coroutine movement;
    private bool firstPoint = true;
    private float spdKf = 1.2f;

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

        nextPoint.y = UnityEngine.Random.Range(0, 1f) >= 0.3f ? nextPoint.y : (ThisTransform.position.y - nextPoint.y) * UnityEngine.Random.Range(0, 8f);

        nextPoint.y = nextPoint.y > 0.7f ? 0.7f : nextPoint.y < -5.5f ? -5.5f : nextPoint.y;

        float dist = Vector2.Distance(nextPoint, ThisTransform.position);

        if (dist <= distanceThreshold || stop)
            return;

        if (dist <= 4 && ThisTransform.position.x < playerTransform.position.x)
            spdKf = 1.1f;
        else if (dist > 4)
            spdKf = 1.2f;

        if (nextPoint.x > ThisTransform.position.x)
            ThisTransform.localScale = new Vector3(-1, 1, 1);
        else
            ThisTransform.localScale = new Vector3(1, 1, 1);

        movement = StartCoroutine(MoveTowards(nextPoint, firstPoint ? speed : speed * spdKf));
        if (firstPoint) triggerSound?.Play();
        firstPoint = false;
    }

    protected override IEnumerator MoveTowards(Vector2 moveTowards, float spd)
    {
        while (Vector2.Distance(moveTowards, ThisTransform.position) > distanceThreshold)
        {
            Vector3 v = Vector2.MoveTowards(ThisTransform.position, moveTowards, spd * Time.deltaTime);
            ThisTransform.position = new Vector3(v.x, v.y, v.y / 10);
            yield return new WaitForEndOfFrame();
        }
        NextPoint();
    }

    protected override IEnumerator BehaviourRoutine()
    {
        while (ThisGameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.3f);
            NextPoint();
        }
    }

    private void OnDisable()
    {
        firstPoint = true;
    }
}
