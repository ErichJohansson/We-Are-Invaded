using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class AreaDamage : MonoBehaviour
{
    public int damage;
    public BoxCollider2D area;
    public LayerMask layerMask;

    private Transform thisTransform;
    private Enemy thisEnemy;

    private void Awake()
    {
        Enemy e = GetComponent<Enemy>();
        if (e != null) e.DieEvent += OnDeath;

        Obstacle o = GetComponent<Obstacle>();
        if (o != null) o.DieEvent += OnDeath;

        thisTransform = transform;
        thisEnemy = GetComponent<Enemy>();
    }

    public void OnDeath(object sender, System.EventArgs eventArgs)
    {
        DealDamageInArea();
    }

    private void DealDamageInArea()
    {
        GameController.Instance.AddBarrel();
        List<Collider2D> overlaped = new List<Collider2D>(Physics2D.OverlapBoxAll(thisTransform.position, area.size, 0, layerMask));
        overlaped.ForEach(x =>
        {
            var e = x.GetComponent<Enemy>();
            if (e != null && e != thisEnemy) e.ReceiveDamage(Random.Range(e.maxHP, damage), e.thisTransform.position, true, false);
        });
    }
}
