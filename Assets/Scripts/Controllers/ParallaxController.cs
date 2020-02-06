using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private float len, startX;

    public GameObject player;
    public float parallaxEffect;

    public void Activate()
    {
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
