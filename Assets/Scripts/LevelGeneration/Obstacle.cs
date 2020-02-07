using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public string spawnerName;
    public RemainsSpawner remainsSpawner;
    public Transform bloodSplatterPos;
    public float bloodTrailLength;
    public Animator animator;

    private Quaternion effectRotation = new Quaternion(180, 0, 0, 1);
    private GameObject effect;
    private GameController gc;

    private void Awake()
    {
        if (createEnemyRemains)
        {
            RemainsSpawner[] spawners = FindObjectsOfType<RemainsSpawner>();
            for (int i = 0; i < spawners.Length; i++)
            {
                if (spawners[i].gameObject.name == spawnerName)
                {
                    remainsSpawner = spawners[i];
                    break;
                }
            }
        }
    }

    void Start()
    {
        gc = FindObjectOfType<GameController>();
        currentHardness = hardness;
        Vector2 pos = gameObject.transform.position;
        gameObject.transform.position = new Vector3(pos.x, pos.y, 9 + (pos.y / 10.00f));
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
            //Debug.Log("got PE");
            if (pe.movementController != null)
            {
                pe.movementController.currentSpeed *= pe.hardness < hardness ? slowAmount * slowAmount : slowAmount;
                if (hardness > pe.hardness)
                    pe.DealDamage(hardness - pe.hardness, gameObject.transform.position);
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
            gc.uc.AddScore(scoreAmount);
        if (createEnemyRemains && remainsSpawner != null)
            remainsSpawner.CreateRemains(new Vector3(bloodSplatterPos.position.x, bloodSplatterPos.position.y, 9 + bloodSplatterPos.position.y / 10f), 15f);

        // use animation of dying instead of particle effect
        if (useAnimationToDie)
        {
            foreach (BoxCollider2D col in GetComponents<BoxCollider2D>())
                col.enabled = false;
            if(animator != null)
            {
                StartCoroutine("AnimationDelay");            
            }
            StopCoroutines();
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

        DestroyManually();
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        animator.SetTrigger("die");
        animator.speed = Random.Range(1f, 1.5f);
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length * 7);
    }

    private void StopCoroutines()
    {
        //Enemy pe = GetComponentInParent<Enemy>();
        //if (pe != null && pe.psController != null)
        //    pe.psController.StopAllCoroutines();
    }

    public void DestroyManually()
    {
        StopCoroutines();
        Destroy(effect);
        Destroy(gameObject);
    }

    public void DisableAdditionalColliders()
    {
        if (personalSpace != null)
            personalSpace.enabled = false;
        if (reachingSpace != null)
            reachingSpace.enabled = false;
    }
}
