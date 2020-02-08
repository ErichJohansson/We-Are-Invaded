using UnityEngine;

public class RepairKit : Modifier
{
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
        DisableAppereance();
        gc.PlayerUnit.Repair(gc.PlayerUnit.maxHP / 2);
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
