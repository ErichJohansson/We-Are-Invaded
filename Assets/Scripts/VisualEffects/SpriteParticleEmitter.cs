using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SpriteParticleEmitter : MonoBehaviour
{
    public int maxParticles;
    public List<Sprite> emittedSprites;

    ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps == null || emittedSprites == null)
            return;
        emittedSprites.ForEach(x => ps.textureSheetAnimation.AddSprite(x));
        ParticleSystem.Burst burst = ps.emission.GetBurst(0);
        burst.count = maxParticles;
    }

    public void Emit(float speed)
    {
        if (ps == null)
            return;
        Debug.Log("emitted");
        ps.Play();
    }
}
