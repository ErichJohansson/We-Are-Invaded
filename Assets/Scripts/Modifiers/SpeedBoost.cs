using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Modifier
{
    private GameController gc;
    private float oldSpeed;
    private float oldSpeedUpRate;

    private void Awake()
    {
        gc = FindObjectOfType<GameController>();
    }

    public override void Activate()
    {
        if (gc.PlayerUnit.SpeedBoost != null)
            return;
        //Application.targetFrameRate = 30;
        gc.follower.gameObject.SetActive(false);
        Activated = true;
        DisableAppereance();
        gc.PlayerUnit.SpeedBoost = this;

        gc.PlayerUnit.MakeInvincible(false); // make invincible and NOT blinking i.e. invincible for the whole speed boost time

        gc.PlayerUnit.FastTravel(lifetime);
        //oldSpeedUpRate = gc.PlayerUnit.speedUpRate;
        //oldSpeed = gc.PlayerUnit.maxSpeed;
        //gc.PlayerUnit.maxSpeed = 80f;
        //gc.PlayerUnit.speedUpRate = 30f;

        gc.uc.AddModifierIcon(icon);
        StartCoroutine("Lifetime");
    }

    public override void Deactivate()
    {
        gc.follower.gameObject.SetActive(true);
        //Application.targetFrameRate = -1;
        //gc.PlayerUnit.maxSpeed = oldSpeed;
        //gc.PlayerUnit.speedUpRate = oldSpeedUpRate;

        gc.PlayerUnit.MakeInvincible(true); // make invincible and blinking i.e. invincible for the short blinking time

        gc.PlayerUnit.SpeedBoost = null;
        gc.uc.RemoveModifierIcon(icon);
        Destroy(gameObject);
    }

    public void Deactivate(bool forceDeactivate)
    {
        //Application.targetFrameRate = -1;
        //gc.PlayerUnit.maxSpeed = oldSpeed;
        //gc.PlayerUnit.speedUpRate = oldSpeedUpRate;
        gc.PlayerUnit.MakeVulnerable(true);
        gc.PlayerUnit.SpeedBoost = null;
        gc.uc.RemoveModifierIcon(icon);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu != null)
        {
            Debug.Log((gc.PlayerUnit.SpeedBoost == null) + " speed boost");
            if (gc.PlayerUnit.SpeedBoost == null)
                Activate();
            else if (!Activated)
            {
                gc.PlayerUnit.SpeedBoost.RenewTime();
                Destroy(gameObject);
            }
        }
    }
}
