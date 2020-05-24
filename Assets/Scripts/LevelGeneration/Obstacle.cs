using System.Collections;
using Assets.Scripts.CustomEventArgs;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public BoxCollider2D personalSpace;
    public BoxCollider2D reachingSpace;

    public int hardness;
    public float slowAmount;
    public bool dealDamageOnDeath;

    public event System.EventHandler<DieEventArgs> DieEvent;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        SetAdditionalCollidersState(false);
        if (sr != null)
            sr.enabled = true;
        Vector2 pos = gameObject.transform.position;
        gameObject.transform.position = new Vector3(pos.x, pos.y, 9 + (pos.y / 10.00f));
    }

    private void OnDisable()
    {
        SetAdditionalCollidersState(true);
    }

    public void SetAdditionalCollidersState(bool state)
    {
        if (personalSpace != null)
            personalSpace.enabled = state;
        if (reachingSpace != null)
            reachingSpace.enabled = state;
    }

    public virtual void OnDeath(DieEventArgs e)
    {
        DieEvent?.Invoke(this, e);
    }
}
