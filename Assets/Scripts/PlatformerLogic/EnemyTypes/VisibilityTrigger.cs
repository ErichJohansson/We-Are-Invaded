using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityTrigger : MonoBehaviour
{
    private EnemyBehaviour enemyBehaviour;

    private void Awake()
    {
        enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyBehaviour != null)
            enemyBehaviour.HandleCollision(collision);
    }
}
