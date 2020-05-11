using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
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
            StartCoroutine("Prewarm");
        }

        public void SetValue(int value)
        {
            if (text == null)
                return;
            StartCoroutine(AnimateText(value));
        }

        public void SetValue(float value)
        {
            if (text == null)
                return;
            StartCoroutine(AnimateText(value));
        }

        private IEnumerator AnimateText(int value)
        {
            int currentValue = 0;
            int deltaValue = value / (int)(time / deltaTime);

            deltaValue = deltaValue == 0 ? 1 : deltaValue;

            while (currentValue < value)
            {
                text.text = prefix + currentValue.ToString() + postfix;
                currentValue += deltaValue;
                yield return new WaitForSecondsRealtime(deltaTime);
            }
            text.text = prefix + value.ToString() + postfix;
        }

        private IEnumerator AnimateText(float value)
        {
            float currentValue = 0;
            float deltaValue = value / time / deltaTime;

            deltaValue = deltaValue == 0 ? 1 : deltaValue;

            while (currentValue < value)
            {
                text.text = prefix + currentValue.ToString() + postfix;
                currentValue += deltaValue;
                yield return new WaitForSecondsRealtime(deltaTime);
            }
            text.text = prefix + value.ToString() + postfix;
        }

        private IEnumerator Prewarm()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            SetValue(0);
        }
    }
}
