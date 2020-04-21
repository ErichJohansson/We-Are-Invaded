using Assets.Scripts.CustomEventArgs;
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
    private BoxCollider2D collider;
    private EnemyBehaviour eb;
    public bool IsInActiveState { get; private set; }

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
        IsInActiveState = false;
        eb = GetComponent<EnemyBehaviour>();
        if(eb != null) eb.EnemyStateChanged += OnEnemyActivatedHanlder;
        GameController.Instance.RestartEvent += Restart;
    }

    public void OnEnemyActivatedHanlder(object sender, EnemyStateEventArgs eventArgs)
    {
        IsInActiveState = eventArgs.state;
        if (obstacle.animator == null)
            return;

        if (eventArgs.state) obstacle.animator.SetTrigger("move");
        else if (!eventArgs.state)
        {
            obstacle.animator.ResetTrigger("idle");
            Debug.Log("set to idle");
        }
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

    private void Restart(object sender, RestartEventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Obstacle obs = collision.GetComponent<Obstacle>();

        if (obs == null)
            return;
        else if (eb != null)
            eb.StopAllCoroutines();

        obstacle.Die();
    }
}
