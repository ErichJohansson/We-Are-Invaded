using UnityEngine;

public class RepairKit : Modifier
{
    public override void Activate()
    {
        if (GameController.Instance.PlayerUnit.SpeedBoost != null)
            return;
        Activated = true;
        DisableAppereance();
        GameController.Instance.PlayerUnit.Repair(GameController.Instance.PlayerUnit.maxHP / 2);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu != null)
        {
            Debug.Log(Activated + " repair kit");
            if (!Activated)
                Activate();
            else
                Destroy(gameObject);
        }
    }
}
