using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
    public PlayerUnit playerUnit;
    public BossToPlayerInteraction playerInteraction;
    public IMovementScenario movementScenario;
    public float turnRate;

    private float bossDistanceX;
    private Vector3 destination;

    void Start()
    {
        playerUnit = FindObjectOfType<PlayerUnit>();
        switch (playerInteraction)
        {
            case BossToPlayerInteraction.HitAndRun:
                movementScenario = new HitAndRunScenario(this, playerUnit);
                break;
            default:
                movementScenario = new HitAndRunScenario(this, playerUnit);
                break;
        }
        StartCoroutine("BehaviourUpdate");
        bossDistanceX = 15f;
    }

    private IEnumerator BehaviourUpdate()
    {
        while(true)
        {
            destination = movementScenario.ReactToPlayerActions();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {
        MoveForward();
        UpdatePosition();
    }

    public void MoveForward()
    { 
        gameObject.transform.position = new Vector3(playerUnit.transform.position.x + bossDistanceX, gameObject.transform.position.y);
    }

    private void UpdatePosition()
    {
        float newY = transform.position.y + (destination.y - transform.position.y) * Time.deltaTime * turnRate;
        transform.position = new Vector3(transform.position.x, newY, 9 + newY / 10.00f);
    }
}
