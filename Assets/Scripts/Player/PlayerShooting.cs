﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public BoxCollider2D splashArea;
    public Transform shootingPoint;
    public GunType gunType;
    public bool auto;
    public int baseDamage;
    public LayerMask layerMask;

    public float reloadTime;
    public float cooldownTime;
    private bool cooldown;

    public float shootingDistance;
    public int magazineCapacity;

    public Animator mainAnimator;
    private ParticleSystem particleSystem;

    public bool Firing { get; set; }
    public bool Reloading { get; private set; }
    public bool InfiniteAmmo { get; set; }
    public int CurrentAmmo { get; private set; }

    private System.Random rnd;

    private void Awake()
    {
        rnd = new System.Random();
        mainAnimator = GetComponent<Animator>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
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
                        ApplyDamage(colliders[i], (int)(baseDamage * (splashArea.size.x / (2 * dist))));
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
                        ApplyDamage(colliders[i], (int)(baseDamage * (splashArea.size.x / (2 * dist))));
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
    #endregion
}
