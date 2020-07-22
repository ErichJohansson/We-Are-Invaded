using System.Collections;
using UnityEngine;

public class RepairKit : Modifier
{
    public override IEnumerator Activate()
    {
        if (GameController.Instance.PlayerUnit.SpeedBoost == null && !Activated)
        {
            Activated = true;
            DisableAppereance();
            int restoreAmount = GameController.Instance.PlayerUnit.maxHP / 2;
            GameController.Instance.PlayerUnit.Repair(restoreAmount);
            GameController.Instance.AddHealthRestored(restoreAmount);
            yield return TriggerNotifier();

            Destroy(gameObject, 3f);
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
                Destroy(gameObject, 3f);
        }
    }
}
