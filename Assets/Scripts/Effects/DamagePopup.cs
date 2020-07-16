using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro textMesh;

    public Color defaultColor;
    public Color criticalColor;

    [SerializeField]
    private float ySpeed = 0.8f;
    private float timeOfLife = 1f;

    private void Setup(float damage, bool crit = false)
    {
        if (crit)
            textMesh.color = criticalColor;
        else
            textMesh.color = defaultColor;

        textMesh.text = damage.ToString();

        StartCoroutine(Lifetime());
    }

    private IEnumerator Lifetime()
    {
        float timeAlive = 0;
        while (timeAlive < timeOfLife)
        {
            timeAlive += Time.deltaTime;
            transform.position += new Vector3(0, ySpeed) * Time.deltaTime;
            textMesh.alpha = 1f - timeAlive;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    public static void CreatePopup(float damage, Vector3 position, bool isCritical = false)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("damagePopup"), position, Quaternion.identity);
        go.GetComponent<DamagePopup>().Setup(damage, isCritical);
    }
}
