using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float acceleration;
    public float topSpeed;

    public float currentSpeed;
    private Vector3 target;

    public void StartMoving()
    {
        if (!gameObject.activeSelf)
            return;
        target = new Vector3(gameObject.transform.position.x - 100f, gameObject.transform.position.y, gameObject.transform.position.z);
        StartCoroutine("Movement");
    }

    private IEnumerator Movement()
    {
        while (true)
        {
            currentSpeed += currentSpeed < topSpeed ? acceleration * Time.deltaTime : 0;
            gameObject.transform.position += new Vector3(-currentSpeed * Time.deltaTime, 0);
            yield return new WaitForEndOfFrame();
        }
    }
}
