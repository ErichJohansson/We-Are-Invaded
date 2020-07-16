using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;

    [SerializeField] private bool loop;

    private void Awake()
    {
        source.loop = loop;
    }

    public void Play()
    {
        source?.PlayOneShot(clip);
    }
}
