using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerShot : MonoBehaviour
{
    public int damage;
    public int splashDamage;
    public float lifetime;
    private Rigidbody2D rb2d;

    //public PlayerShooting playerShooting;
    //public PlatformerShooting platformerShooting;

    public IShotRecycler shotRecycler;

    public string effectName;

    public GameObject effect;
    private Quaternion effectRotation = Quaternion.identity;

    public float baseShotPower;
    public float[] initialY; // [0]-top Y  [1]-bottom Y

    public bool playerControlled;
    public float parentHeight;

    public CircleCollider2D splashCollider;

    public bool colliding;

    void Start()
    {
        SpawnEffect();
        if (rb2d == null)
            rb2d = GetComponent<Rigidbody2D>();

        //shotRecycler = platformerShooting;
        //if (platformerShooting == null)
        //{
        //    shotRecycler = playerShooting;
        //}
    }

    void SpawnEffect()
    {
        GameObject toSpawn = Resources.Load<GameObject>(effectName);
        if (toSpawn == null)
            return;
        effect = Instantiate(toSpawn, gameObject.transform.position, effectRotation, gameObject.transform.parent);
        effect.SetActive(false);
    }

    public void Activate()
    {
        //initialY = new float[] { gameObject.transform.position.y + 0.05f, gameObject.transform.position.y - 0.05f };
        if(rb2d == null)
            rb2d = GetComponent<Rigidbody2D>();
        if (rb2d.gravityScale == 0)
            StartCoroutine("Lifetime");
        else
            StartCoroutine("YDependentLifetime");
    }

    private IEnumerator Lifetime()
    {
        float t = 0f;
        while(t < lifetime)
        {
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Recycle(true);
    }

    private IEnumerator YDependentLifetime()
    {
        float groundLevel = initialY[1] - parentHeight;
        while (gameObject.transform.position.y > groundLevel)
            yield return new WaitForEndOfFrame();
        Recycle(true);
    }

    /// <summary>
    /// Collision of SHOT and OBSTACLE or PLAYER or PLATFORMER ENEMY  
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<GroundDecor>() != null || colliding)
            return;

        colliding = true;
        Vector3 lastPos = gameObject.transform.position;

        StopCoroutine("Lifetime");
        Recycle(false);

        PlayerUnit player = col.gameObject.GetComponentInParent<PlayerUnit>();
        Enemy pe = col.gameObject.GetComponent<Enemy>();

        if (player != null)
        {
            player.DealDamage(damage, lastPos, false);
        }

        if (pe != null)
        {
            pe.DealDamage(damage, lastPos, playerShot: playerControlled, isCritical: true);
        }
    }

    public void Recycle(bool useSplash)
    {
        gameObject.SetActive(false);
        ExplosionEffect.UseEffect(gameObject.transform.position, effect, reusable: true);

        if(useSplash && splashCollider != null)
        {
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(gameObject.transform.position, splashCollider.radius);
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                Enemy platformerEnemy = collider2Ds[i].GetComponent<Enemy>();
                if (platformerEnemy != null)
                    platformerEnemy.DealDamage(splashDamage, collider2Ds[i].transform.position, playerShot: playerControlled);
            }
        }

        if (rb2d == null)
            rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.zero;

        if(shotRecycler != null)
            shotRecycler.RecycleShot(this);
    }
}
