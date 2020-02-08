using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public BoxCollider2D splashArea;
    public Transform shootingPoint;
    public GunType gunType;
    public int baseDamage;

    public float reloadTime;
    public float cooldownTime;
    private bool cooldown;

    public float shootingDistance;
    public int magazineCapacity;

    private GameController gc;

    public Animator mainAnimator;
    private ParticleSystem particleSystem;

    [Header("Effects")]
    public Animator hitEffectAnimator;

    public bool Firing { get; set; }
    public bool Reloading { get; private set; }
    public int CurrentAmmo { get; private set; }
    public bool InfiniteAmmo { get; set; }

    private void Awake()
    {
        gc = FindObjectOfType<GameController>();
        mainAnimator = GetComponent<Animator>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        if(!Reloading)
        gc.uc.UpdateReloadSlider(1f);
        gc.uc.UpdateAmmo(CurrentAmmo);
    }

    private void Update()
    {
        if (Firing)
            PlayerFire();
    }

    private void PlayerFire()
    {
        if (Reloading || cooldown || !gameObject.activeSelf)
            return;

        if(!InfiniteAmmo)
            CurrentAmmo--;
        gc.uc.UpdateAmmo(CurrentAmmo);

        Fire();

        StartCoroutine("Cooldown");

        if (CurrentAmmo <= 0)
            StartCoroutine("Reload");
    }

    private void ApplyDamage(Collider2D col, int damage)
    {
        if (Random.Range(0, 1f) <= 0.15f)
            return;
        Enemy pe;
        if (col.TryGetComponent(out pe))
        {
            pe.DealDamage(damage, pe.transform.position, playerShot: true, isCritical: damage > baseDamage);
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
        if (hitEffectAnimator != null)
            hitEffectAnimator.SetTrigger("shotStart");

        switch (gunType)
        {
            case GunType.SemiAuto:
                Collider2D[] colliders = Physics2D.OverlapBoxAll(shootingPoint.transform.position, splashArea.size, 0, ~0);

                if (colliders.Length == 0)
                {
                    Debug.Log("col = null");
                    return;
                }

                new List<Collider2D>(colliders).
                    ForEach(col => ApplyDamage(col, (int)(baseDamage * (splashArea.size.x / (2 * Vector2.Distance(gameObject.transform.position, col.transform.position))))));
                break;
        }
    }

    public void Restart()
    {
        StopAllCoroutines();

        CurrentAmmo = magazineCapacity;
        gc.uc.UpdateAmmo(CurrentAmmo);
        gc.uc.UpdateReloadSlider(1f);
        cooldown = false;
        Reloading = false;
        InfiniteAmmo = false;
    }

    #region coroutines
    private IEnumerator Reload()
    {
        float t = 0;
        Reloading = true;
        while (t < reloadTime)
        {
            t += Time.deltaTime;
            gc.uc.UpdateReloadSlider(t / reloadTime);
            yield return new WaitForEndOfFrame();
        }
        Reloading = false;
        CurrentAmmo = magazineCapacity;
        gc.uc.UpdateAmmo(CurrentAmmo);
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
    #endregion
}
