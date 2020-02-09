using System.Collections;
using UnityEngine;

public class Regenerator : MonoBehaviour
{
    private LevelPart parent;
    private bool regenerating;

    void Start()
    {
        parent = GetComponentInParent<LevelPart>();    
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (regenerating)
            return;
        PlayerUnit player = collider.gameObject.GetComponentInParent<PlayerUnit>();
        if (player == null)
            return;
        regenerating = true;
        StartCoroutine("RegenerationCooldown");
        parent.Regenerate();
    }

    private IEnumerator RegenerationCooldown()
    {
        yield return new WaitForSeconds(1f);
        regenerating = false;
    }
}
