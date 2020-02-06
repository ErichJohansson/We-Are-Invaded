using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDecor : MonoBehaviour
{
    public BoxCollider2D personalSpace;
    public BoxCollider2D reachingSpace;
    public string effectName;

    private Quaternion effectRotation = new Quaternion(180, 0, 0, 1);
    private GameObject effect;

    private void Start()
    {
        Vector2 pos = gameObject.transform.position;
        gameObject.transform.position = new Vector3(pos.x, pos.y, 9 + (pos.y / 10.00f));

        if (personalSpace != null)
            personalSpace.enabled = false;
        if (reachingSpace != null)
            reachingSpace.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        float diePosZ = -14.881337f; // !!! КОСТЫЛЬ !!!     

        Enemy pe = null;
        PlayerUnit pu = null;

        if (col.gameObject.transform.parent != null && col.gameObject.transform.parent.gameObject.TryGetComponent(out pu))
        {
            Debug.Log("collided with player");
            diePosZ = pu.gameObject.transform.position.z;
        }
        else if (col.gameObject.TryGetComponent(out pe))
        {
            Debug.Log("got PE");
            diePosZ = pe.gameObject.transform.position.z;
        }

        if (diePosZ == -14.881337f) // !!! КОСТЫЛЬ !!!
            return;

        Die(diePosZ);
    }

    public void Die(float diePosZ)
    {
        // use particle effect
        if (effectName != "")
        {
            if (diePosZ > gameObject.transform.position.z)
                ExplosionEffect.CreateEffect(gameObject.transform.position, effectName, gameObject.transform.parent.parent.parent, effectRotation);
            else
                ExplosionEffect.CreateEffect(gameObject.transform.position, effectName, gameObject.transform.parent.parent.parent);
        }

        DestroyManually();
    }

    private void StopCoroutines()
    {
        //Enemy pe = GetComponentInParent<Enemy>();
        //if (pe != null && pe.psController != null)
        //    pe.psController.StopAllCoroutines();
    }

    public void DestroyManually()
    {
        StopCoroutines();
        Destroy(effect);
        Destroy(gameObject);
    }
}
