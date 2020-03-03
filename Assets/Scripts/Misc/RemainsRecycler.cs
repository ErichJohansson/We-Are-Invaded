using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainsRecycler : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine("Lifetime");
    }

    private void OnDisable()
    {
        StopCoroutine("Lifetime");
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(15f);
        gameObject.SetActive(false);
    }
}
