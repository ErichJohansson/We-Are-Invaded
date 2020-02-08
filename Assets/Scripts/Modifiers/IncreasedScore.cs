using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedScore : Modifier
{
    private GameController gc;

    private void Awake()
    {
        gc = FindObjectOfType<GameController>();
    }

    public override void Activate()
    {
        Activated = true;
        DisableAppereance();
        gc.PlayerUnit.IncreasedScore = this;
        gc.PlayerUnit.scoreModifier = 2f;
        gc.uc.AddModifierIcon(icon);
        StartCoroutine("Lifetime");
    }

    public override void Deactivate()
    {
        gc.PlayerUnit.IncreasedScore = null;
        gc.PlayerUnit.scoreModifier = 1f;
        gc.uc.RemoveModifierIcon(icon);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu != null)
        {
            Debug.Log((gc.PlayerUnit.IncreasedScore == null) + " score");
            if (gc.PlayerUnit.IncreasedScore == null)
                Activate();
            else if (!Activated)
            {
                gc.PlayerUnit.IncreasedScore.RenewTime();
                Destroy(gameObject);
            }
        }
    }
}
