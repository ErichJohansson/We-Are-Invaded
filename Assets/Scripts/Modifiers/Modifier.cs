using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier : MonoBehaviour
{
    public float lifetime;
    public GameObject visualEffect;
    public Sprite icon;

    private float timeRemains;
    private bool isIndependent;

    public virtual void Deactivate() { }
    public virtual void Activate() { }

    protected void RenewTime()
    {
        timeRemains = lifetime;
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
        timeRemains = lifetime;
        if (!isIndependent)
            MakeIndependent();

        while (timeRemains > 0)
        {
            timeRemains -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Deactivate();
    }

    protected void DisableAppereance()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;
    }
}
