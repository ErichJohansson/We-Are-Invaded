using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIObject : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve animationCurve;
    [SerializeField]
    private Vector3 turnOffPosition;
    [SerializeField]
    private Vector3 turnOnPosition;
    [SerializeField]
    float speed;

    public bool instant;

    private RectTransform rectTransform;

    public float waitFor;

    public bool isShown;
    private bool isMoving;

    private GameController gc;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        gc = FindObjectOfType<GameController>();
    }

    public void ActivatePanel()
    {
        if (isShown)
            HidePanel();
        else
            ShowPanel();
    }

    public void ShowPanel()
    {
        if(instant)
        {
            rectTransform.anchoredPosition = turnOnPosition;
            gc.someScreenIsShown = true;
            isShown = true;
            isMoving = false;
            return;
        }
        if (!isMoving)
            StartCoroutine(MoveObject(turnOffPosition, turnOnPosition, true));
    }

    public void HidePanel()
    {
        if (instant)
        {
            rectTransform.anchoredPosition = turnOffPosition;
            gc.someScreenIsShown = false;
            isShown = false;
            isMoving = false;
            return;
        }
        if (!isMoving)
            StartCoroutine(MoveObject(turnOnPosition, turnOffPosition, false));
    }

    IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, bool finalState)
    {
        isMoving = true;
        float t = 0;
        while (t < 1)
        {
            t += waitFor * speed;
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, animationCurve.Evaluate(t));
            yield return new WaitForSecondsRealtime(waitFor);
        }
        isShown = finalState;
        isMoving = false;
        gc.someScreenIsShown = finalState;
    }
}
