﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    private GameController gc;
    private Camera cam;

    float halfWidth;

    void Start()
    {
        halfWidth = Screen.width / 2;
        cam = Camera.main;
        gc = FindObjectOfType<GameController>();
        if(gc.playerObject != null)
            gc.playerObject.transform.position = new Vector3(gc.playerObject.transform.position.x, gc.playerObject.transform.position.y, -gc.playerObject.transform.position.y / 10f);
    }

    void Update()
    {
        if (gc.Pause && Input.GetKeyDown(KeyCode.Escape))
        {
            gc.uc.pauseScreen.BackToMainMenu();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            gc.uc.pauseScreen.ActivatePause();

        if (gc.PlayerUnit.IsFastTraveling)
            return;

        if (Input.GetMouseButton(0))
            HandleMousePlatformer();

        if (Input.GetMouseButtonUp(0))
            StopCoroutine("ChangingPosition");
    }

    void HandleMousePlatformer()
    {
        if (Time.timeScale == 0)
            return;

        Vector3 mousePos = Input.touchSupported ? Input.touchCount > 1 ? GetRightTouch() : Input.mousePosition : Input.mousePosition;

        if (mousePos.x > halfWidth)
        {
            gc.PlayerUnit.MoveTowards(cam.ScreenToWorldPoint(mousePos));
        }
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
