using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class SpeedBoost : Modifier
{
    public override IEnumerator Activate()
    {
        if (GameController.Instance.PlayerUnit.SpeedBoost == null && !Activated)
        {
            // do visuals
            Activated = true;
            DisableAppereance();
            UIController.Instance.AddModifierIcon(icon);
            Follower.Instance.gameObject.SetActive(false);

            yield return TriggerNotifier();

            CameraShakeController.Instance.ShakeCamera(lifetime, 0.8f, 2f);
            GameController.Instance.PlayerUnit.SpeedBoost = this;
            GameController.Instance.PlayerUnit.MakeInvincible(false); // make invincible and NOT blinking i.e. invincible for the whole speed boost time
            GameController.Instance.PlayerUnit.FastTravel(lifetime);
            StartCoroutine("Lifetime");
        }
        else
            yield return new WaitForEndOfFrame();
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
                StartCoroutine(Activate());
            else if (!Activated)
            {
                GameController.Instance.PlayerUnit.SpeedBoost.RenewTime();
                Destroy(gameObject);
            }
        }
    }
}
