using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier : MonoBehaviour
{
    public float lifetime;
    public GameObject visualEffect;
    public Sprite icon;

    private bool isIndependent;

    public float TimeRemains { get; private set; }
    public bool Activated { get; protected set; }

    public virtual void Deactivate() { }
    public virtual void Activate() { }

    protected void RenewTime()
    {
        TimeRemains = lifetime;
    }

    private void MakeIndependent()
    {
        StageStrip ss = GetComponentInParent<StageStrip>();
        if (ss != null)
        {
            isIndependent = ss.spawnedObjects.Remove(gameObject);
            transform.parent = transform.parent.parent;
        }
    }

    protected IEnumerator Lifetime()
    {
        TimeRemains = lifetime;
        if (!isIndependent)
            MakeIndependent();

        while (TimeRemains > 0)
        {
            TimeRemains -= Time.deltaTime;
            Debug.Log(TimeRemains);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("over");
        Deactivate();
    }

    protected void DisableAppereance()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;
    }
}
