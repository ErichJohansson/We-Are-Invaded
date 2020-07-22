using UnityEngine;

public class PhysicalCollisionHandler : MonoBehaviour
{
    public AudioPlayer bumpSound;
    public AudioPlayer scratchSound;

    private PlayerUnit parent;
    private float speedReduction;
    private float speedThreshold;

    private void Start()
    {
        parent = GetComponentInParent<PlayerUnit>();
        speedReduction = parent.maxSpeed * 0.01f;
        speedThreshold = parent.maxSpeed * 0.05f;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (parent.currentSpeed <= speedThreshold)
            return;
        SlowDown();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        scratchSound?.Stop();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bumpSound?.Play();
        scratchSound?.Play();
    }

    public void SlowDown()
    {
        parent.currentSpeed -= speedReduction;
    }
}
