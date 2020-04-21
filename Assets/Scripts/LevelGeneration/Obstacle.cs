using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public BoxCollider2D personalSpace;
    public BoxCollider2D reachingSpace;
    public BoxCollider2D mainCollider;

    public int hardness;

    public float slowAmount;

    [Header("Score & Dying")]
    public bool useAnimationToDie;
    public bool addScoreOnDeath;
    public int scoreAmount;

    public bool hierarchyObject;
    public bool forceY;
    public float forcedY;

    public bool isModifier;

    [Header("Visual Effects")]
    public bool createRemains;
    public bool disableSpriteRendererOnDeath;
    public string[] allowedRemainsTags;
    public Transform bloodSplatterPos;
    public float bloodTrailLength;
    public Animator animator;

    private ParticleSystem ps;
    private SpriteParticleEmitter spriteEmitter;
    private SpriteRenderer sr;
    private ObjectPooler pooler;

    private void Awake()
    {
        if (createRemains)
        {
            pooler = FindObjectOfType<ObjectPooler>();
        }

        ps = GetComponentInChildren<ParticleSystem>();
        spriteEmitter = GetComponent<SpriteParticleEmitter>();
        sr = GetComponent<SpriteRenderer>();

        if (ps != null)
        {
            ParticleSystem.Burst burst = ps.emission.GetBurst(0);
            burst.minCount = (short)(2 * scoreAmount);
            burst.maxCount = (short)(5 * scoreAmount);
        }
    }

    private void OnEnable()
    {
        SetAdditionalCollidersState(false);
        if (sr != null)
            sr.enabled = true;
        Vector2 pos = gameObject.transform.position;
        gameObject.transform.position = new Vector3(pos.x, pos.y, 9 + (pos.y / 10.00f));
        if (mainCollider != null)
            mainCollider.enabled = true;
    }

    private void OnDisable()
    {
        SetAdditionalCollidersState(true);
    }

    public void Die()
    {
        if (mainCollider != null)
            mainCollider.enabled = false;

        if (isModifier)
            return;

        if (spriteEmitter != null)
            spriteEmitter.Emit(GameController.Instance.PlayerUnit.currentSpeed);

        if (disableSpriteRendererOnDeath && sr != null)
            sr.enabled = false;

        if (addScoreOnDeath)
        {
            UIController.Instance.AddScore(scoreAmount);
            if (ps != null) ps.Play();
        }

        if (createRemains && allowedRemainsTags.Length != 0)
        {
            GameObject obj = pooler.GetPooledObject(allowedRemainsTags[Random.Range(0, allowedRemainsTags.Length)]);
            obj.transform.position = new Vector3(bloodSplatterPos.position.x, bloodSplatterPos.position.y, 9 + bloodSplatterPos.position.y / 10f);
            obj.SetActive(true);
            StartCoroutine(DeactivateAfterDelay(15f, obj));
        }

        if (useAnimationToDie)
        {
            foreach (BoxCollider2D col in GetComponents<BoxCollider2D>())
                col.enabled = false;
            if(animator != null)
                StartCoroutine(DeathDelay());
        }
        else
        {
            StartCoroutine(DeathDelay(false));
        }

    }

    private IEnumerator DeathDelay(bool basedOnAmination = true)
    {
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        if (basedOnAmination)
        {
            animator.SetTrigger("die");
            animator.speed = Random.Range(1f, 1.5f);
            DeactivateAfterDelay(animator.GetCurrentAnimatorStateInfo(0).length * 7, gameObject);
        }
        else
            DeactivateAfterDelay(6f, gameObject);
    }

    public void SetAdditionalCollidersState(bool state)
    {
        if (personalSpace != null)
            personalSpace.enabled = state;
        if (reachingSpace != null)
            reachingSpace.enabled = state;
    }

    private IEnumerator DeactivateAfterDelay(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
