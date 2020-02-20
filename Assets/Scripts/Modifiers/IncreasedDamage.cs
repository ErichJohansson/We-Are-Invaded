using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedDamage : Modifier
{
    private int oldDmg;

    public override void Activate()
    {
        if (GameController.Instance.PlayerUnit.SpeedBoost != null)
            return;
        Activated = true;
        // do visuals
        DisableAppereance();
        UIController.Instance.AddModifierIcon(icon);

        GameController.Instance.PlayerUnit.IncreasedDamage = this;
        StartCoroutine("Lifetime");
        oldDmg = GameController.Instance.PlayerUnit.shooting.baseDamage;
        GameController.Instance.PlayerUnit.shooting.baseDamage = oldDmg * 4;
    }

    public override void Deactivate()
    {
        GameController.Instance.PlayerUnit.IncreasedDamage = null;
        UIController.Instance.RemoveModifierIcon(icon);
        GameController.Instance.PlayerUnit.shooting.baseDamage = oldDmg;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu != null)
        {
            Debug.Log((GameController.Instance.PlayerUnit.IncreasedDamage == null) + " damage");
            if (GameController.Instance.PlayerUnit.IncreasedDamage == null)
                Activate();
            else if(!Activated)
            {
                GameController.Instance.PlayerUnit.IncreasedDamage.RenewTime();
                Destroy(gameObject);
            }
        }
    }
}
