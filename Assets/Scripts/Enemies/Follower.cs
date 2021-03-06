﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float initialDeltaX;
    public float distanceToStopFollowing;
    public float speedModifier;
    [Range(0, 1f)] public float maxSpeedPercentage;

    [Header("Warning Settings")]
    public GameObject dangerSign;
    public float blinkTime;

    private float distanceToPlayer;
    private float speedThreshold;
    private bool isFollowing;
    private Vector3 startPos = new Vector3(-200, 0, 0);
    private bool wokeUp;
    private AudioPlayer roar;

    public static Follower Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        roar = GetComponent<AudioPlayer>();
    }

    public void StartFollowing()
    {
        speedThreshold = GameController.Instance.PlayerUnit.maxSpeed * maxSpeedPercentage;
        isFollowing = false;
        wokeUp = false;
        StartCoroutine("Delay");
    }

    public void Restart()
    {
        StopCoroutine("DangerSign");
        StopCoroutine("Delay");
        gameObject.transform.position = startPos;
        isFollowing = false;
        wokeUp = false;
        dangerSign.SetActive(false);
        StartCoroutine("Delay");
    }

    private void Update()
    {
        if (GameController.Instance.PlayerUnit == null || GameController.Instance.Pause || !wokeUp)
        {
            roar?.Stop();
            return;
        }

        if (isFollowing)
        {
            distanceToPlayer = GameController.Instance.PlayerUnit.gameObject.transform.position.x - gameObject.transform.position.x;
            if (distanceToPlayer > distanceToStopFollowing)
            {
                roar?.Stop();
                StopCoroutine("DangerSign");
                dangerSign.SetActive(false);
                isFollowing = false;
                return;
            }

            gameObject.transform.position += new Vector3(speedModifier * Time.deltaTime * GameController.Instance.PlayerUnit.maxSpeed, 0, 0);
        }
        else if(GameController.Instance.PlayerUnit.currentSpeed < speedThreshold)
        {
            isFollowing = true;
            StartCoroutine("DangerSign");
            gameObject.transform.position = new Vector3(GameController.Instance.PlayerUnit.transform.position.x - initialDeltaX, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerUnit pu = collision.GetComponentInParent<PlayerUnit>();
        if (pu != null)
        {
            pu.ReceiveDamage(999, startPos, false);
        }
    }

    private IEnumerator DangerSign()
    {
        roar?.Play();
        while(true)
        {
            dangerSign.SetActive(!dangerSign.activeSelf);
            yield return new WaitForSeconds(blinkTime * (distanceToPlayer / initialDeltaX));
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(10f);
        wokeUp = true;
    }
}
