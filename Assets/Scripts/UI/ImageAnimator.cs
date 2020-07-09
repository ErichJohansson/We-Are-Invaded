using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimator : MonoBehaviour
{
    public AnimationClip clip;
    public Sprite[] sprites;
    private int frame = 0;
    private Image image;
    private float delay;
    bool allowStart;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (clip == null || image == null) return;
        allowStart = sprites.Length != 0;
        delay = 1f / clip.frameRate;
    }

    public void Play()
    {
        if (!allowStart) Awake();
        if (allowStart) StartCoroutine("Animate");
    }

    public void Stop()
    {
        StopCoroutine("Animate");
    }

    private IEnumerator Animate()
    {
        yield return new WaitForSecondsRealtime(1f);
        while (true)
        {
            if (frame >= sprites.Length) frame = 0;
            image.sprite = sprites[frame];
            frame++;
            yield return new WaitForSecondsRealtime(delay);
        }
    }
}
