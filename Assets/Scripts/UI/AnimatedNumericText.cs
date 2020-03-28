using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedNumericText : MonoBehaviour
{
    [Range(0.5f, 2f)] public float time;
    [Range(0, 0.2f)] public float deltaTime;
    public string prefix;
    public string postfix;

    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void SetValue(dynamic value)
    {
        if (text == null)
            return;
        StartCoroutine(AnimateText(value));
    }

    private IEnumerator AnimateText(dynamic value)
    {
        dynamic currentValue = 0;
        dynamic deltaValue;

        if (value.GetType() == typeof(int))
            deltaValue = value / (int)(time / deltaTime);
        else
            deltaValue = value / (float)(time / deltaTime);

        if (deltaValue == 0)
            deltaValue = 1;

        while (currentValue < value)
        {
            text.text = prefix + currentValue.ToString() + postfix;
            currentValue += deltaValue;
            yield return new WaitForSecondsRealtime(deltaTime);
        }
        text.text = prefix + value.ToString() + postfix;
    }
}
