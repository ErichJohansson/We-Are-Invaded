using Assets.Scripts.CustomEventArgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DamageRecieverComponent
{
    public EnemyShooting enemyShooting;
    public Animator hitAnimator;
    [HideInInspector] public Transform thisTransform;

    private BoxCollider2D collider;
    private EnemyBehaviour eb;
    private int currentHP;
    [HideInInspector] public Animator animator;

    public bool IsInActiveState { get; private set; }

    public event System.EventHandler<DieEventArgs> DieEvent;

    private void Awake()
    {
        enemyShooting = GetComponent<EnemyShooting>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponentInChildren<Animator>();
        eb = GetComponent<EnemyBehaviour>();
        currentHP = maxHP;
        thisTransform = transform;
    }

    private void OnEnable()
    {
        collider.enabled = true;
        IsInActiveState = false;
        if (eb != null) eb.EnemyStateChanged += OnEnemyActivatedHanlder;
        GameController.Instance.RestartEvent += Restart;
        currentHP = maxHP;
    }

    public virtual void OnDeath(DieEventArgs e)
    {
        DieEvent?.Invoke(this, e);
        if (eb != null)
        {
            eb.StopAllCoroutines();
            eb.ForceStop();
        }
        if (enemyShooting != null)
            enemyShooting.Stop();
    }

    public void OnEnemyActivatedHanlder(object sender, EnemyStateEventArgs eventArgs)
    {
        IsInActiveState = eventArgs.state;
        if (animator == null)
            return;

        if (eventArgs.state) animator.SetTrigger("move");
        else if (!eventArgs.state)
        {
            animator.ResetTrigger("idle");
        }
    }

    public override void ReceiveDamage(int damage, Vector3 pos, bool hitByPlayer, bool isCritical = false)
    {
        DamagePopup.CreatePopup(damage, pos, isCritical);
        currentHP -= damage;
        if (hitByPlayer)
            GameController.Instance.AddDamageToEnemies(damage);
        if (currentHP <= 0)
        {
            OnDeath(new DieEventArgs(hitByPlayer));
        }
        else if (hitAnimator != null)
            StartCoroutine("AnimationDelay");
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.3f));
        hitAnimator.SetTrigger("hit");
        hitAnimator.speed = Random.Range(1f, 1.5f);
    }

    private void Restart(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Obstacle obs = collision.GetComponent<Obstacle>();

        if (obs == null)
            return; 

        OnDeath(new DieEventArgs(false));
    }
}
