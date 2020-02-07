using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float initialDeltaX;
    public float distanceToStopFollowing;
    public float speedModifier;

    [Header("Warning Settings")]
    public GameObject dangerSign;
    public float blinkTime;

    private float distanceToPlayer;
    private float speedThreshold;
    private bool isFollowing;
    private Vector3 startPos = new Vector3(-200, 0, 0);
    private GameController gc;

    private void Awake()
    {
        gc = FindObjectOfType<GameController>();
    }

    public void StartFollowing()
    {
        speedThreshold = gc.PlayerUnit.maxSpeed / 3f;
    }

    public void Restart()
    {
        gameObject.transform.position = startPos;
        isFollowing = false;
    }

    private void Update()
    {
        if (gc.PlayerUnit == null || gc.Pause)
            return;

        if (isFollowing)
        {
            distanceToPlayer = gc.PlayerUnit.gameObject.transform.position.x - gameObject.transform.position.x;
            if (distanceToPlayer > distanceToStopFollowing)
            {
                StopCoroutine("DangerSign");
                dangerSign.SetActive(false);
                isFollowing = false;
                return;
            }

            gameObject.transform.position += new Vector3(speedModifier * Time.deltaTime * gc.PlayerUnit.maxSpeed, 0, 0);
        }
        else if(gc.PlayerUnit.currentSpeed < speedThreshold)
        {
            isFollowing = true;
            StartCoroutine("DangerSign");
            gameObject.transform.position = new Vector3(gc.PlayerUnit.transform.position.x - initialDeltaX, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu != null)
        {
            pu.DealDamage(99999, startPos);
        }
    }

    private IEnumerator DangerSign()
    {
        while(true)
        {
            dangerSign.SetActive(!dangerSign.activeSelf);
            yield return new WaitForSeconds(blinkTime * (distanceToPlayer / initialDeltaX));
        }
    }
}
