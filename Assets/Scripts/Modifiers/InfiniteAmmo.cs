using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteAmmo : Modifier
{
    private int oldCap;

    public override void Activate()
    {
        if (GameController.Instance.PlayerUnit.SpeedBoost != null)
            return;
        Activated = true;
        // do visuals
        DisableAppereance();

        //set icon
        UIController.Instance.AddModifierIcon(icon);

        GameController.Instance.PlayerUnit.InfiniteAmmo = this;
        StartCoroutine("Lifetime");
        oldCap = GameController.Instance.PlayerUnit.shooting.magazineCapacity;
        GameController.Instance.PlayerUnit.shooting.magazineCapacity = 9999;
        GameController.Instance.PlayerUnit.shooting.Restart();
        GameController.Instance.PlayerUnit.shooting.InfiniteAmmo = true;
    }

    public override void Deactivate()
    {
        GameController.Instance.PlayerUnit.InfiniteAmmo = null;
        UIController.Instance.RemoveModifierIcon(icon);
        GameController.Instance.PlayerUnit.shooting.magazineCapacity = oldCap;
        GameController.Instance.PlayerUnit.shooting.Restart();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if(pu != null)
        {
            Debug.Log((GameController.Instance.PlayerUnit.InfiniteAmmo == null) + " ammo");
            if (GameController.Instance.PlayerUnit.InfiniteAmmo == null)
                Activate();
            else if(!Activated)
            {
                GameController.Instance.PlayerUnit.InfiniteAmmo.RenewTime();
                Destroy(gameObject);
            }
        }
    }
}
