using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Modifier : MonoBehaviour
{
    public float lifetime;
    public GameObject visualEffect;
    public Sprite icon;
    public Sprite effectImage;
    public float effectTimeScale;
    private bool isIndependent;

    private Light2D light;

    public float TimeRemains { get; private set; }
    public bool Activated { get; protected set; }

    public virtual void Deactivate() { }
    public virtual IEnumerator Activate() { yield return new WaitForEndOfFrame(); }

    private void OnEnable()
    {
        StartCoroutine("LightCycle");
    }

    private void OnDisable()
    {
        StopCoroutine("LightCycle");
    }

    private void Awake()
    {
        light = GetComponentInChildren<Light2D>();
    }

    protected Coroutine TriggerNotifier()
    {
        return UIController.Instance.ActivateModifierEffect(effectImage, effectTimeScale);
    }

    protected void RenewTime()
    {
        TimeRemains = lifetime;
    }

    private void MakeIndependent()
    {
        Stripe ss = GetComponentInParent<Stripe>();
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
            yield return new WaitForEndOfFrame();
        }
        Deactivate();
    }

    protected IEnumerator LightCycle()
    {
        bool reverse = false;
        while (light != null)
        {
            light.intensity += reverse ? -0.1f : 0.1f;
            if (light.intensity >= 3f) reverse = true;
            if (light.intensity <= 0.01f) reverse = false;
            yield return new WaitForSeconds(0.01f);
        }
    }

    protected void DisableAppereance()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;
        StopCoroutine("LightCycle");
        light.intensity = 0;
    }
}
