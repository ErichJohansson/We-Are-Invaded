using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int mtth;
    private int ticks;
    ObjectPooler op;
    UIController ui;
    PlayerUnit player;

    public static BossController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        op = FindObjectOfType<ObjectPooler>();
        ui = UIController.Instance;
        StartCoroutine("BossTimer");
    }

    private IEnumerator BossTimer()
    {
        player = GameController.Instance.PlayerUnit;
        while (Random.Range(0, 1f) > ticks / (float)mtth || player.IsFastTraveling || ui.ShowingEffect)
        {
            ticks++;
            yield return new WaitForSeconds(1f);
        }
        GameObject go = op.GetPooledObject("boss");
        go.SetActive(true);
        Vector3 playerPos = GameController.Instance.PlayerUnit.transform.position;
        go.transform.position = new Vector3(playerPos.x + 25, playerPos.y);
        go.GetComponent<Vertiboss>().ActivateBoss();
        ticks = 0;
    }

    public void Restart()
    {
        StopCoroutine("BossTimer");
        ticks = 0;
        StartCoroutine("BossTimer");
    }

}
