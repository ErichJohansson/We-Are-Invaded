using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class InfiniteAmmo : Modifier
{
    private int oldCap;

    public override IEnumerator Activate()
    {
        if (GameController.Instance.PlayerUnit.SpeedBoost == null && !Activated)
        {
            // do visuals
            Activated = true;
            DisableAppereance();
            UIController.Instance.AddModifierIcon(icon);

            yield return TriggerNotifier();

            GameController.Instance.PlayerUnit.InfiniteAmmo = this;
            StartCoroutine("Lifetime");
            oldCap = GameController.Instance.PlayerUnit.shooting.magazineCapacity;
            GameController.Instance.PlayerUnit.shooting.magazineCapacity = 9999;
            GameController.Instance.PlayerUnit.shooting.Restart();
            GameController.Instance.PlayerUnit.shooting.InfiniteAmmo = true;
        }
        else
            yield return new WaitForEndOfFrame();
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
                StartCoroutine(Activate());
            else if(!Activated)
            {
                GameController.Instance.PlayerUnit.InfiniteAmmo.RenewTime();
                Destroy(gameObject);
            }
        }
    }
}
