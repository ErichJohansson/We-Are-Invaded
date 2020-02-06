using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    private Vector3 parentPos;
    public ParticleSystem particleSystem;
    private float timeOfLife;
    private float timeAlive = 0;

    private void Setup(bool reusable = false, Animator animator = null) 
    {
        gameObject.SetActive(true);
        parentPos = gameObject.transform.position;

        particleSystem.Play();
    }

    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        gameObject.transform.position = parentPos;
    }

    #region Effects
    public static void CreateEffect(Vector3 pos, string effectName, Transform parent, Quaternion rotation)
    {
        GameObject toSpawn = Resources.Load<GameObject>(effectName);
        if (toSpawn == null)
            return;
        GameObject go = Instantiate(toSpawn, pos, rotation, parent);
        go.GetComponent<ExplosionEffect>().Setup();
    }

    public static void CreateEffect(Vector3 pos, string effectName, Transform parent)
    {
        GameObject toSpawn = Resources.Load<GameObject>(effectName);
        if (toSpawn == null)
            return;
        GameObject go = Instantiate(toSpawn, pos, Quaternion.identity, parent);
        go.GetComponent<ExplosionEffect>().Setup();
    }

    public static void UseEffect(Vector3 useAt, GameObject effect, bool reusable = false)
    {
        if (effect == null)
            return;
        effect.transform.position = useAt;
        effect.GetComponent<ExplosionEffect>().Setup(reusable);
    }

    public static void UseEffect(Vector3 useAt, GameObject effect, Transform newParent, bool reusable = false)
    {
        effect.transform.position = useAt;
        effect.transform.parent = newParent;
        effect.GetComponent<ExplosionEffect>().Setup(reusable);
    }

    public static void UseEffect(Vector3 useAt, GameObject effect, Transform newParent, Quaternion newRotation, bool reusable = false)
    {
        effect.transform.rotation = newRotation;
        effect.transform.position = useAt;
        effect.transform.parent = newParent;
        effect.GetComponent<ExplosionEffect>().Setup(reusable);
    }
    #endregion
}
