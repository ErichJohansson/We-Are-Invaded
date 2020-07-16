using UnityEngine;

public class PhysicalCollisionHandler : MonoBehaviour
{
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
        parent.currentSpeed -= speedReduction;
    }
}
