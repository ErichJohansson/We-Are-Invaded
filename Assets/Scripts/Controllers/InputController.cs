﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class InputController : MonoBehaviour
{
    private Camera cam;
    private float halfWidth;
    private bool inputRecieved;

    void Awake()
    {
        halfWidth = Screen.width / 2;
        cam = Camera.main;
        if(GameController.Instance.playerObject != null)
            GameController.Instance.playerObject.transform.position = new Vector3(GameController.Instance.playerObject.transform.position.x, GameController.Instance.playerObject.transform.position.y, -GameController.Instance.playerObject.transform.position.y / 10f);
    }

    void Update()
    {
        if (GameController.Instance.Pause && Input.GetKeyDown(KeyCode.Escape))
        {
            UIController.Instance.pauseScreen.ActivatePause();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            UIController.Instance.pauseScreen.ActivatePause();

        if (GameController.Instance.PlayerUnit == null || GameController.Instance.PlayerUnit.IsFastTraveling || UIController.Instance.Loading)
            return;

        if (Input.GetMouseButton(0))
            inputRecieved = true;

        if (Input.GetMouseButtonUp(0))
        {
            inputRecieved = false;
            StopTurning();
        }
    }

    private void FixedUpdate()
    {
        if (inputRecieved)
            HandleMouse();
        inputRecieved = false;
    }

    private void HandleMouse()
    {
        if (Time.timeScale == 0)
            return;

        Vector3 mousePos = Input.touchSupported ? Input.touchCount > 1 ? GetRightTouch() : Input.mousePosition : Input.mousePosition;

        if (mousePos.x > halfWidth)
        {
            GameController.Instance.PlayerUnit.MoveTowards(cam.ScreenToWorldPoint(mousePos));
        }
    }

    private void StopTurning()
    {
        GameController.Instance.PlayerUnit.StopTurning();
    }

    private Vector3 GetRightTouch()
    {
        if (!Input.touchSupported)
            return Vector3.zero;

        Touch rightTouch = Input.touches[0];
        float prevX = -5000;

        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Input.touches[i].position.x > prevX)
            {
                rightTouch = Input.touches[i];
                prevX = rightTouch.position.x;
            }
        }

        return new Vector3(rightTouch.position.x, rightTouch.position.y, 0);
    }
}
