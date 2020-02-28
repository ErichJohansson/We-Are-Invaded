using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP;
    private int currentHP;

    public int hardness;

    public EnemyMovement movementController;
    public EnemyShooting enemyShooting;

    public Animator hitAnimator;

    public bool isBoss;

    private Obstacle obstacle;

    private void Awake()
    {
        enemyShooting = GetComponent<EnemyShooting>();
        obstacle = GetComponent<Obstacle>();
    }

    private void OnEnable()
    {
        currentHP = maxHP;
    }

    public void ReceiveDamage(int damage, Vector3 pos, bool isCritical = false, bool playerShot = false)
    {
        if (!playerShot)
            return;

        DamagePopup.CreatePopup(damage, pos, isCritical);
        currentHP -= damage;
        if (currentHP <= 0)
        {
            if (obstacle != null)
                obstacle.Die(gameObject.transform.position.y);
            else
                Destroy(gameObject);

            if (enemyShooting != null)
            {
                enemyShooting.Stop();
            }
        }
        else if (hitAnimator != null)
        {
            StartCoroutine("AnimationDelay");
        }
        if(isBoss)
            Debug.Log(currentHP);
    }

    public void Restart()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
            col.enabled = true;
    }

    public static void SetActiveEnemies(bool state)
    {
        StageStrip[] stageStrips = FindObjectsOfType<StageStrip>();
        for (int i = 0; i < stageStrips.Length; i++)
        {
            for (int j = 0; j < stageStrips[i].spawnedUnits.Count; j++)
            {
                if (stageStrips[i].spawnedUnits[j] == null)
                    continue;
                stageStrips[i].spawnedUnits[j].gameObject.SetActive(state);
                if (state) stageStrips[i].spawnedUnits[j].Restart();
            }
        }
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.3f));
        hitAnimator.SetTrigger("hit");
        hitAnimator.speed = Random.Range(1f, 1.5f);
    }
}
