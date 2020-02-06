using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outliner : MonoBehaviour
{
    Material material;
    public float totalTime;
    public Color color;

    private Coroutine pulseCoroutine;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        material.SetColor("_OutlineColor", color);
    }

    public void StartGlowing()
    {
        if(pulseCoroutine == null)
            pulseCoroutine = StartCoroutine("Pulsate");
    }

    public void StopGlowing()
    {
        StopCoroutine(pulseCoroutine);
        StartCoroutine("DisableGlow");
        pulseCoroutine = null;
        
    }

    private IEnumerator Pulsate()
    {
        float currentTime = 0;
        bool increase = true;
        while (true)
        {
            currentTime += increase ? Time.deltaTime : -Time.deltaTime;
            material.SetFloat("_OutlineThickness", currentTime >= 0 ? currentTime : 0 / totalTime);
            if (currentTime >= totalTime)
            {
                increase = false;
            }
            if (currentTime <= 0)
            {
                increase = true;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator DisableGlow()
    {
        float currentTime = material.GetFloat("_OutlineThickness");
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            material.SetFloat("_OutlineThickness", currentTime >= 0 ? currentTime : 0 / totalTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
