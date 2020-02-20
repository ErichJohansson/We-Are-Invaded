using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerShootingPhysical : MonoBehaviour, IShotRecycler
{
    public Transform firePoint;
    public List<GameObject> availableShots; // render them on the screen according to this list
    public int currentShotID;

    public float cooldownTime;
    public float reloadTime;

    public int magazineCapacity;
    public int currentAmmo;
    public int totalAmmo;

    public int baseDamage;
    public float shotPower;

    private List<PlatformerShot> spawnedShots;
    private List<PlatformerShot> freeShots;

    public bool Reloading { get; private set; }
    private bool cooldown;

    public float height;

    public BoxCollider2D shootingCollider;

    void Start()
    {
        spawnedShots = new List<PlatformerShot>();
        freeShots = new List<PlatformerShot>();
        for (int i = 0; i < availableShots.Count; i++)
        {
            for (int j = 0; j < 2 * magazineCapacity; j++)
            {
                spawnedShots.Add(Instantiate(availableShots[i], firePoint.transform.position, Quaternion.identity, gameObject.transform.parent.parent).GetComponent<PlatformerShot>());
                spawnedShots[spawnedShots.Count - 1].playerControlled = true;
                spawnedShots[spawnedShots.Count - 1].shotRecycler = this;
                freeShots.Add(spawnedShots[spawnedShots.Count - 1]);
                spawnedShots[spawnedShots.Count - 1].playerControlled = this;
                spawnedShots[spawnedShots.Count - 1].parentHeight = height;
                spawnedShots[spawnedShots.Count - 1].damage = baseDamage + spawnedShots[spawnedShots.Count - 1].damage;
                spawnedShots[spawnedShots.Count - 1].gameObject.SetActive(false);
            }
        }

        currentAmmo = magazineCapacity;
        UIController.Instance.UpdateReloadSlider(1f);
    }

    public void RecycleShot(PlatformerShot sender)
    {
        //sender.damageDealt = false;
        sender.transform.position = firePoint.position;
        freeShots.Add(sender);
    }

    public void PlayerFire()
    {
        if (Reloading || cooldown || !gameObject.activeSelf)
            return;

        currentAmmo--;

        Fire();

        StartCoroutine("Cooldown");

        if (currentAmmo <= 0)
            StartCoroutine("Reload");
    }


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
        currentAmmo = magazineCapacity;
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

    private void Fire()
    {
        // TODO: apply animation
        //animator.SetTrigger("shotStart");
        Debug.Log("FIRE");
        if(freeShots.Count == 0)
        {
            spawnedShots.Add(Instantiate(availableShots[0], firePoint.transform.position, Quaternion.identity, gameObject.transform.parent.parent).GetComponent<PlatformerShot>());
            spawnedShots[spawnedShots.Count - 1].playerControlled = true;
            //spawnedShots[spawnedShots.Count - 1].playerShooting = this;
            freeShots.Add(spawnedShots[spawnedShots.Count - 1]);
            spawnedShots[spawnedShots.Count - 1].gameObject.SetActive(false);
        }

        freeShots[0].gameObject.transform.position = firePoint.position;
        freeShots[0].gameObject.SetActive(true);
        freeShots[0].colliding = false;
        freeShots[0].Activate();
        Rigidbody2D shotRB = freeShots[0].gameObject.GetComponent<Rigidbody2D>();
        float shotStrength = freeShots[0].baseShotPower * shotPower;
        shotRB.AddForce(new Vector2(shotStrength < 0 ? -shotStrength : shotStrength, 0), ForceMode2D.Impulse);
        freeShots.Remove(freeShots[0]);
    }

    private void FireNonPhysical()// ignores obstacles
    {
        // TODO: apply animation
        //animator.SetTrigger("shotStart");
        Debug.Log("FIRE");

        new List<Collider2D>(Physics2D.OverlapBoxAll(firePoint.position, shootingCollider.size, 0)).ForEach(x => ApplyDamage(x));
    }

    private void ApplyDamage(Collider2D col)
    {
        Enemy pe;
        if (col.TryGetComponent(out pe))
            pe.DealDamage(baseDamage, pe.transform.position);

    }

    public void Restart()
    {
        StopAllCoroutines();
        UIController.Instance.UpdateReloadSlider(0);
        freeShots.Clear();
        currentAmmo = magazineCapacity;
        cooldown = false;
        Reloading = false;
        for (int i = 0; i < spawnedShots.Count; i++)
        {
            spawnedShots[i].gameObject.SetActive(false);
            spawnedShots[i].gameObject.transform.position = firePoint.position;
            spawnedShots[i].gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            freeShots.Add(spawnedShots[i]);
        }

        //StartCoroutine("BurstRoutine");
    }
}
