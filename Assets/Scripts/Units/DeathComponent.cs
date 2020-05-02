using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeactivateComponent))]
public class DeathComponent : MonoBehaviour
{
    public bool disableSpriteRendererOnDeath;

    public float bloodTrailLength;
    public Animator animator;

    public bool useAnimationToDie;
    private SpriteRenderer sr;
    private DeactivateComponent deactivateComponent;

    private void Awake()
    {
        Enemy e = GetComponent<Enemy>();
        if (e != null) e.DieEvent += OnDeath;

        Obstacle o = GetComponent<Obstacle>();
        if (o != null) o.DieEvent += OnDeath;

        sr = GetComponent<SpriteRenderer>();
        deactivateComponent = GetComponent<DeactivateComponent>();


        animator = GetComponentInChildren<Animator>();
        if(animator == null) animator = GetComponent<Animator>();
    }

    public void OnDeath(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("die comp");
        Die();
    }

    private void Die()
    {
        GameController.Instance.PlayerUnit.trailLifetime += bloodTrailLength;
        if (disableSpriteRendererOnDeath && sr != null)
            sr.enabled = false;
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
                animator.speed = Random.Range(1f, 1.5f);
                deactivateComponent.DeactivateAfterDelay(animator.GetCurrentAnimatorStateInfo(0).length * 7, gameObject);
            }
            else
                deactivateComponent.DeactivateAfterDelay(6f, gameObject);
        }
    }
}
