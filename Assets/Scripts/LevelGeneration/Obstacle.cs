using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public BoxCollider2D personalSpace;
    public BoxCollider2D reachingSpace;

    public int hardness;
    private int currentHardness;

    public float slowAmount;
    public string effectName;

    [Header("Score & Dying")]
    public bool useAnimationToDie;
    public bool addScoreOnDeath;
    public int scoreAmount;

    public bool hierarchyObject;
    public bool forceY;
    public float forcedY;

    public bool isModifier;

    [Header("Visual Effects")]
    public bool createPopUp;
    public bool createEnemyRemains;
    public string[] allowedRemainsTags;
    public Transform bloodSplatterPos;
    public float bloodTrailLength;
    public Animator animator;

    private Quaternion effectRotation = new Quaternion(180, 0, 0, 1);
    private GameObject effect;
    private ObjectPooler pooler;

    private void Awake()
    {
        currentHardness = hardness;
        if (createEnemyRemains)
        {
            pooler = FindObjectOfType<ObjectPooler>();
        }
    }

    private void OnEnable()
    {
        Vector2 pos = gameObject.transform.position;
        gameObject.transform.position = new Vector3(pos.x, pos.y, 9 + (pos.y / 10.00f));
        SetAdditionalCollidersActive(false);
    }


    private void OnDisable()
    {
        SetAdditionalCollidersActive(true);
    }

    /// <summary>
    /// Collision of OBSTACLE and OTHER OBSTACLE or PLAYER 
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (hierarchyObject)
            return;

        float diePosZ = -14.881337f; // !!! КОСТЫЛЬ !!!     
        
        Enemy pe = null;
        PlatformerShot ps = null;

        if (col.gameObject.TryGetComponent(out ps))
        {
            if (!ps.playerControlled)
                return;
            if(createPopUp)
                DamagePopup.CreatePopup(ps.damage, ps.gameObject.transform.position);
            currentHardness -= ps.damage;
            if (currentHardness <= 0)
                diePosZ = ps.gameObject.transform.position.z;
        }
        else if (col.gameObject.TryGetComponent(out pe))
        {
            if (col.GetComponent<Enemy>() == null)
                return;
            if (pe.movementController != null)
            {
                pe.movementController.currentSpeed *= pe.hardness < hardness ? slowAmount * slowAmount : slowAmount;
                if (hardness > pe.hardness)
                    pe.ReceiveDamage(hardness - pe.hardness, gameObject.transform.position);
                diePosZ = pe.gameObject.transform.position.z;
            }
        }

        if (diePosZ == -14.881337f) // !!! КОСТЫЛЬ !!!
            return;

        Die(diePosZ);
    }

    public void Die(float diePosZ)
    {
        if (isModifier)
            return;

        if(addScoreOnDeath)
            UIController.Instance.AddScore(scoreAmount);
        if (createEnemyRemains && allowedRemainsTags.Length != 0)
        {
            GameObject obj = pooler.GetPooledObject(allowedRemainsTags[Random.Range(0, allowedRemainsTags.Length)]);
            obj.transform.position = new Vector3(bloodSplatterPos.position.x, bloodSplatterPos.position.y, 9 + bloodSplatterPos.position.y / 10f);
            obj.SetActive(true);
            StartCoroutine(DeactivateAfterDelay(15f, obj));
        }

        // use animation of dying instead of particle effect
        if (useAnimationToDie)
        {
            foreach (BoxCollider2D col in GetComponents<BoxCollider2D>())
                col.enabled = false;
            if(animator != null)
            {
                StartCoroutine("AnimationDelay");            
            }
            return;
        }

        // use particle effect
        if (effectName != "")
        {
            if (diePosZ > gameObject.transform.position.z)
                ExplosionEffect.CreateEffect(gameObject.transform.position, effectName, gameObject.transform.parent.parent.parent, effectRotation);
            else
                ExplosionEffect.CreateEffect(gameObject.transform.position, effectName, gameObject.transform.parent.parent.parent);
        }

        gameObject.SetActive(false);
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        animator.SetTrigger("die");
        animator.speed = Random.Range(1f, 1.5f);
        DeactivateAfterDelay(animator.GetCurrentAnimatorStateInfo(0).length * 7, gameObject);
    }

    public void SetAdditionalCollidersActive(bool state)
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
