using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for emmiting such things as road cone
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class SpriteParticleEmitter : MonoBehaviour
{
    public int maxParticles;
    public List<Sprite> emittedSprites;

    ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        Enemy e = GetComponent<Enemy>();
        if (e != null) e.DieEvent += OnDeath;
        Obstacle o = GetComponent<Obstacle>();
        if (o != null) o.DieEvent += OnDeath;

        if (ps == null || emittedSprites == null)
            return;
        emittedSprites.ForEach(x => ps.textureSheetAnimation.AddSprite(x));
        ParticleSystem.Burst burst = ps.emission.GetBurst(0);
        burst.count = maxParticles;
    }


    public void OnDeath(object sender, System.EventArgs eventArgs)
    {
        Emit();
    }

    public void Emit()
    {
        if (ps == null)
            return;
        ps.Play();
    }
}
