﻿using System.Collections;
using UnityEngine;

public class RepairKit : Modifier
{
    public override IEnumerator Activate()
    {
        if (GameController.Instance.PlayerUnit.SpeedBoost == null && !Activated)
        {
            Activated = true;
            DisableAppereance();
            GameController.Instance.PlayerUnit.Repair(GameController.Instance.PlayerUnit.maxHP / 2);

            yield return TriggerNotifier();

            Destroy(gameObject);
        }
        else
            yield return new WaitForEndOfFrame();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu != null)
        {
            Debug.Log(Activated + " repair kit");
            if (!Activated)
                StartCoroutine(Activate());
            else
                Destroy(gameObject);
        }
    }
}
