using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimator : MonoBehaviour
{
    public AnimationClip clip;
    private int frame = 0;
    private Image image;
    private List<Sprite> sprites;
    private float delay;
    bool allowStart;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (clip == null || image == null) return;
        UpdateClip();
        allowStart = sprites.Count != 0;
        delay = 1f / clip.frameRate;
    }

    public void Play()
    {
        if(!allowStart) Awake();
        if (allowStart) StartCoroutine("Animate");
    }

    public void Stop()
    {
        StopCoroutine("Animate");
    }

    public void UpdateClip()
    {
        sprites = GetSpritesFromClip(clip);
    }

    private IEnumerator Animate()
    {
        yield return new WaitForSecondsRealtime(1f);
        while (true)
        {
            if (frame >= sprites.Count) frame = 0;
            image.sprite = sprites[frame];
            frame++;
            yield return new WaitForSecondsRealtime(delay);
        }
    }

    private List<Sprite> GetSpritesFromClip(AnimationClip clip)
    {
        var sprites = new List<Sprite>();
        if (clip != null)
        {
            foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                foreach (var frame in keyframes)
                {
                    sprites.Add((Sprite)frame.value);
                }
            }
        }
        return sprites;
    }
}
