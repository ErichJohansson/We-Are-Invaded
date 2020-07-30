using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioPlayer monsterDeathSound;

    public static SoundController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
