using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteAmmo : Modifier
{
    private int oldCap;

    private GameController gc;

    private void Awake()
    {
        gc = FindObjectOfType<GameController>();
    }

    public override void Activate()
    {
        Activated = true;
        // do visuals
        DisableAppereance();

        //set icon
        gc.uc.AddModifierIcon(icon);

        gc.PlayerUnit.InfiniteAmmo = this;
        StartCoroutine("Lifetime");
        oldCap = gc.PlayerUnit.shooting.magazineCapacity;
        gc.PlayerUnit.shooting.magazineCapacity = 9999;
        gc.PlayerUnit.shooting.Restart();
        gc.PlayerUnit.shooting.InfiniteAmmo = true;
    }

    public override void Deactivate()
    {
        gc.PlayerUnit.InfiniteAmmo = null;
        gc.uc.RemoveModifierIcon(icon);
        gc.PlayerUnit.shooting.magazineCapacity = oldCap;
        gc.PlayerUnit.shooting.Restart();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if(pu != null)
        {
            Debug.Log((gc.PlayerUnit.InfiniteAmmo == null) + " ammo");
            if (gc.PlayerUnit.InfiniteAmmo == null)
                Activate();
            else if(!Activated)
            {
                gc.PlayerUnit.InfiniteAmmo.RenewTime();
                Destroy(gameObject);
            }
        }
    }
}
