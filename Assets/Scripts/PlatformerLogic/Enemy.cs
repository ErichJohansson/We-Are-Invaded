using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP;
    private int currentHP;

    public int hardness;

    public EnemyShooting enemyShooting;

    public Animator hitAnimator;

    public bool isBoss;

    private Obstacle obstacle;
    BoxCollider2D collider;

    private void Awake()
    {
        enemyShooting = GetComponent<EnemyShooting>();
        obstacle = GetComponent<Obstacle>();
        collider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        currentHP = maxHP;
        collider.enabled = true;
    }

    public void ReceiveDamage(int damage, Vector3 pos, bool isCritical = false, bool playerShot = false)
    {
        if (!playerShot)
            return;

        DamagePopup.CreatePopup(damage, pos, isCritical);
        currentHP -= damage;
        if (currentHP <= 0)
        {
            if (obstacle != null)
                obstacle.Die();

            if (enemyShooting != null)
            {
                enemyShooting.Stop();
            }
        }
        else if (hitAnimator != null)
        {
            StartCoroutine("AnimationDelay");
        }
        if(isBoss)
            Debug.Log(currentHP);
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.3f));
        hitAnimator.SetTrigger("hit");
        hitAnimator.speed = Random.Range(1f, 1.5f);
    }
}
