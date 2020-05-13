using System.Collections;
using UI;
using UnityEngine;

public class IncreasedDamage : Modifier
{
    private int oldDmg;

    public override IEnumerator Activate()
    {
        if (GameController.Instance.PlayerUnit.SpeedBoost == null && !Activated)
        {
            // do visuals
            Activated = true;
            DisableAppereance();
            UIController.Instance.AddModifierIcon(icon);

            yield return TriggerNotifier();

            GameController.Instance.PlayerUnit.IncreasedDamage = this;
            StartCoroutine("Lifetime");
            oldDmg = GameController.Instance.PlayerUnit.shooting.BaseDamage;
            GameController.Instance.PlayerUnit.shooting.BaseDamage = oldDmg * 4;
        }
        else
            yield return new WaitForEndOfFrame();
    }

    public override void Deactivate()
    {
        GameController.Instance.PlayerUnit.IncreasedDamage = null;
        UIController.Instance.RemoveModifierIcon(icon);
        GameController.Instance.PlayerUnit.shooting.BaseDamage = oldDmg;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu != null)
        {
            Debug.Log((GameController.Instance.PlayerUnit.IncreasedDamage == null) + " damage");
            if (GameController.Instance.PlayerUnit.IncreasedDamage == null)
                StartCoroutine(Activate());
            else if(!Activated)
            {
                GameController.Instance.PlayerUnit.IncreasedDamage.RenewTime();
                Destroy(gameObject);
            }
        }
    }
}
