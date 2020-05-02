using System.Collections;
using UnityEngine;

public class DeactivateComponent : MonoBehaviour
{
    public IEnumerator DeactivateAfterDelay(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
