using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DeactivateComponent : MonoBehaviour
{
    public List<GameObject> additionalObjectsWithDelay;
    public List<GameObject> additionalObjectsWithoutDelay;

    public void DeactivateAfterDelay(float delay, GameObject obj)
    {
        StartCoroutine(DeactivateRoutine(delay, obj));
    }

    private IEnumerator DeactivateRoutine(float delay, GameObject obj)
    {
        additionalObjectsWithoutDelay.ForEach(x => x.SetActive(false));
        yield return new WaitForSeconds(delay);
        additionalObjectsWithDelay.ForEach(x => x.SetActive(false));
        obj.SetActive(false);
    }
}
