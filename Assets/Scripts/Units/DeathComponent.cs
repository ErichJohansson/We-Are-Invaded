﻿using System.Collections;
using Assets.Scripts.CustomEventArgs;
using UnityEngine;

[RequireComponent(typeof(DeactivationComponent))]
public class DeathComponent : MonoBehaviour
{
    public bool disableSpriteRendererOnDeath;

    public float bloodTrailLength;
    public Animator animator;
    public AudioPlayer deathSound;

    public bool useAnimationToDie;
    private SpriteRenderer sr;
    private DeactivationComponent deactivateComponent;

    private void Awake()
    {
        Enemy e = GetComponent<Enemy>();
        if (e != null) e.DieEvent += OnDeath;

        Obstacle o = GetComponent<Obstacle>();
        if (o != null) o.DieEvent += OnDeath;

        sr = GetComponent<SpriteRenderer>();
        deactivateComponent = GetComponent<DeactivationComponent>();

        animator = GetComponentInChildren<Animator>();
        if(animator == null) animator = GetComponent<Animator>();
    }

    public void OnDeath(object sender, DieEventArgs e)
    {
        Die(e.KilledByPlayer);
    }

    private void Die(bool hitByPlayer)
    {
        if(hitByPlayer) GameController.Instance.PlayerUnit.trailLifetime += bloodTrailLength;
        if (disableSpriteRendererOnDeath && sr != null)
            sr.enabled = false;
        deathSound?.Play();
        if (useAnimationToDie)
        {
            foreach (BoxCollider2D col in GetComponents<BoxCollider2D>())
                col.enabled = false;
            if (animator != null)
                StartCoroutine(DeathDelay());
        }
        else
            StartCoroutine(DeathDelay(false));
    }

    private IEnumerator DeathDelay(bool basedOnAmination = true)
    {
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        if (deactivateComponent != null)
        {
            if (basedOnAmination)
            {
                animator.SetTrigger("die");
                deactivateComponent.DeactivateAfterDelay(animator.GetCurrentAnimatorStateInfo(0).length * 7, gameObject);
            }
            else
                deactivateComponent.DeactivateAfterDelay(1f, gameObject);
        }
    }
}
