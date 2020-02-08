using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedDamage : Modifier
{
    private int oldDmg;
    private GameController gc;

    private void Awake()
    {
        gc = FindObjectOfType<GameController>();
    }

    public override void Activate()
    {
        if (gc.PlayerUnit.SpeedBoost != null)
            return;
        Activated = true;
        // do visuals
        DisableAppereance();
        gc.uc.AddModifierIcon(icon);

        gc.PlayerUnit.IncreasedDamage = this;
        StartCoroutine("Lifetime");
        oldDmg = gc.PlayerUnit.shooting.baseDamage;
        gc.PlayerUnit.shooting.baseDamage = oldDmg * 4;
    }

    public override void Deactivate()
    {
        gc.PlayerUnit.IncreasedDamage = null;
        gc.uc.RemoveModifierIcon(icon);
        gc.PlayerUnit.shooting.baseDamage = oldDmg;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu != null)
        {
            Debug.Log((gc.PlayerUnit.IncreasedDamage == null) + " damage");
            if (gc.PlayerUnit.IncreasedDamage == null)
                Activate();
            else if(!Activated)
            {
                gc.PlayerUnit.IncreasedDamage.RenewTime();
                Destroy(gameObject);
            }
        }
    }
}
