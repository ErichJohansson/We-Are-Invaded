using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class PlayerShooting : MonoBehaviour
{
    public BoxCollider2D splashArea;
    public Transform shootingPoint;
    public GunType gunType;
    public bool auto;
    public LayerMask layerMask;

    public float reloadTime;
    public float cooldownTime;
    private bool cooldown;

    public float shootingDistance;
    public int magazineCapacity;

    [Header("Gun Light settings")]
    public GameObject gunLightSource;
    [Range(0f, 1f)] public float gunLightLength;

    [HideInInspector] public Animator mainAnimator;
    private ParticleSystem particleSystem;

    private int defaultDmg;
    private int baseDmg;

    public bool Firing { get; set; }
    public bool Reloading { get; private set; }
    public bool InfiniteAmmo { get; set; }
    public int CurrentAmmo { get; private set; }
    public int BaseDamage { 
        get {
            return baseDmg;
        }
        set {
            defaultDmg = baseDmg;
            baseDmg = value;
            if (defaultDmg == 0)
                defaultDmg = baseDmg;
        } 
    }

    private System.Random rnd;

    private void Awake()
    {
        rnd = new System.Random();
        mainAnimator = GetComponent<Animator>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        defaultDmg = BaseDamage;
    }

    private void Start()
    {
        if (!Reloading)
        {
            UIController.Instance.UpdateReloadSlider(1f);
            UIController.Instance.UpdateAmmo(CurrentAmmo);
        }
    }

    private void Update()
    {
        if (Firing)
            PlayerFire();
    }

    private void PlayerFire()
    {
        if (Reloading || cooldown || !gameObject.activeSelf || GameController.Instance.PlayerUnit.IsFastTraveling)
            return;

        if(!InfiniteAmmo)
            CurrentAmmo--;
        UIController.Instance.UpdateAmmo(CurrentAmmo);

        Fire();

        Firing = auto;
        StartCoroutine("Cooldown");
        if (CurrentAmmo <= 0)
            StartCoroutine("Reload");
    }

    private void ApplyDamage(Collider2D col, int damage)
    {
        Enemy pe;
        if (col.TryGetComponent(out pe))
        {
            pe.ReceiveDamage(damage, pe.transform.position, false, isCritical: damage > BaseDamage);
            SpawnEffect(col);
        }
    }

    private void SpawnEffect(Collider2D col)
    {
        SpriteRenderer sr = col.GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
            return;

        float y = sr.bounds.center.y + Random.Range(-sr.bounds.extents.y, sr.bounds.extents.y);
        float x = sr.bounds.center.x - sr.bounds.extents.x + 0.3f;
    }

    private void Fire()
    {
        if(mainAnimator != null)
            mainAnimator.SetTrigger("shotStart");
        if (particleSystem != null)
            particleSystem.Play();
        if (defaultDmg < baseDmg)
            CameraShakeController.Instance.ShakeCamera(0.2f, 2, 1);

        StartCoroutine("GunLight");
        List<Collider2D> colliders = new List<Collider2D>(Physics2D.OverlapBoxAll(shootingPoint.transform.position, splashArea.size, 0, layerMask));
        
        if (colliders.Count == 0)
            return;
        colliders = colliders.OrderBy(x => rnd.Next()).ToList();

        switch (gunType)
        {
            case GunType.Shotgun:
                for (int i = 0; i < colliders.Count; i++)
                {
                    float dist = Vector2.Distance(new Vector3(shootingPoint.transform.position.x - splashArea.size.x, shootingPoint.transform.position.y, shootingPoint.transform.position.z), 
                        colliders[i].transform.position);
                    float hitThreshold = splashArea.size.x / dist;
                    float hitNow = hitThreshold >= 1f ? 1f : Random.Range(0, 1f);
                    if(hitNow <= hitThreshold)
                        ApplyDamage(colliders[i], (int)(BaseDamage * (splashArea.size.x / (2 * dist))));
                }
                break;
            case GunType.SingleShot:
                for (int i = 0; i < colliders.Count; i++)
                {
                    float dist = Vector2.Distance(new Vector3(shootingPoint.transform.position.x - splashArea.size.x, shootingPoint.transform.position.y, shootingPoint.transform.position.z),
                        colliders[i].transform.position);
                    float hitThreshold = splashArea.size.x / dist;
                    float hitNow = hitThreshold >= 1f ? 1f : Random.Range(0, 1f);
                    if (hitNow <= hitThreshold)
                    {
                        ApplyDamage(colliders[i], (int)(BaseDamage * (splashArea.size.x / (2 * dist))));
                        break;
                    }
                }
                break;
        }
    }

    public void Restart()
    {
        StopAllCoroutines();

        CurrentAmmo = magazineCapacity;
        UIController.Instance.UpdateAmmo(CurrentAmmo);
        UIController.Instance.UpdateReloadSlider(1f);
        cooldown = false;
        Reloading = false;
        InfiniteAmmo = false;
        gunLightSource.SetActive(false);
    }

    #region coroutines
    private IEnumerator Reload()
    {
        float t = 0;
        Reloading = true;
        while (t < reloadTime)
        {
            t += Time.deltaTime;
            UIController.Instance.UpdateReloadSlider(t / reloadTime);
            yield return new WaitForEndOfFrame();
        }
        Reloading = false;
        CurrentAmmo = magazineCapacity;
        UIController.Instance.UpdateAmmo(CurrentAmmo);
    }

    private IEnumerator Cooldown()
    {
        float t = 0;
        cooldown = true;
        while (t < cooldownTime)
        {
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        cooldown = false;
    }

    private IEnumerator GunLight()
    {
        gunLightSource.SetActive(true);
        yield return new WaitForSeconds(gunLightLength);
        gunLightSource.SetActive(false);
    }
    #endregion
}
