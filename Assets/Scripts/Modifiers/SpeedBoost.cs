using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Modifier
{
    public override void Activate()
    {
        if (GameController.Instance.PlayerUnit.SpeedBoost != null)
            return;

        Follower.Instance.gameObject.SetActive(false);
        Activated = true;
        DisableAppereance();
        GameController.Instance.PlayerUnit.SpeedBoost = this;

        GameController.Instance.PlayerUnit.MakeInvincible(false); // make invincible and NOT blinking i.e. invincible for the whole speed boost time

        GameController.Instance.PlayerUnit.FastTravel(lifetime);

        UIController.Instance.AddModifierIcon(icon);
        StartCoroutine("Lifetime");
    }

    public override void Deactivate()
    {
        Follower.Instance.gameObject.SetActive(true);

        GameController.Instance.PlayerUnit.MakeInvincible(true); // make invincible and blinking i.e. invincible for the short blinking time

        GameController.Instance.PlayerUnit.SpeedBoost = null;
        UIController.Instance.RemoveModifierIcon(icon);
        Destroy(gameObject);
    }

    public void ForceDeactivation()
    {
        GameController.Instance.PlayerUnit.MakeVulnerable(true);
        GameController.Instance.PlayerUnit.SpeedBoost = null;
        UIController.Instance.RemoveModifierIcon(icon);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu != null)
        {
            Debug.Log((GameController.Instance.PlayerUnit.SpeedBoost == null) + " speed boost");
            if (GameController.Instance.PlayerUnit.SpeedBoost == null)
                Activate();
            else if (!Activated)
            {
                GameController.Instance.PlayerUnit.SpeedBoost.RenewTime();
                Destroy(gameObject);
            }
        }
    }
}
