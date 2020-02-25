using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public GameObject player;
    public float parallaxEffect;

    private float len, startX;
    private Vector3 initialPosition;

    public void Activate()
    {
        if (initialPosition == Vector3.zero)
            initialPosition = gameObject.transform.position;
        else
            gameObject.transform.position = initialPosition;
        startX = transform.position.x;
        len = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        if (player == null)
            return;
        float temp = player.transform.position.x * (1 - parallaxEffect);
        float dist = player.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startX + dist, transform.position.y, transform.position.z);

        if (temp > startX + len)
            startX += len;
        else if (temp < startX - len)
            startX -= len;
    }
}
