using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAndRunScenario : IMovementScenario
{
    private MovementBehaviour movementBehaviour;
    private PlayerUnit playerUnit;

    public HitAndRunScenario(MovementBehaviour movementBehaviour, PlayerUnit playerUnit) {
        this.movementBehaviour = movementBehaviour;
        this.playerUnit = playerUnit;
    }

    public Vector3 ReactToPlayerActions()
    {
        if (playerUnit == null)
            return Vector3.zero;

        if (playerUnit.shooting.Reloading)
        {
            return playerUnit.transform.position;
        }
        else
        {
            return CalculateDirection();
        }
    }

    /// <summary>
    /// Calculates where Boss should move according to player's actions
    /// </summary>
    /// <returns>Position where the boss should be moving</returns>
    private Vector3 CalculateDirection()
    {
        float rndVal = Random.Range(-2f, 2f);
        if (playerUnit.transform.position.y > movementBehaviour.transform.position.y)
        {
            // BOSS is below player
            if (playerUnit.Direction > 0) 
                return new Vector3(0, movementBehaviour.transform.position.y + rndVal * 2f); // Player is moving UP
            else if (movementBehaviour.transform.position.y + rndVal * 2f < 4f && movementBehaviour.transform.position.y + rndVal * 2f > -4f)
                return new Vector3(0, movementBehaviour.transform.position.y - 1f > -4f ?
                    movementBehaviour.transform.position.y - 1f : movementBehaviour.transform.position.y + 2f); // Player is moving DOWN
            else
                return new Vector3(0, movementBehaviour.transform.position.y);
        }
        else
        {
            // BOSS is above player
            if (playerUnit.Direction > 0)
                return new Vector3(0, movementBehaviour.transform.position.y + 1f < 4f ?
                    movementBehaviour.transform.position.y + 1f : movementBehaviour.transform.position.y - 2f); // Player is moving UP
            else if (movementBehaviour.transform.position.y + rndVal * 2f < 4f && movementBehaviour.transform.position.y + rndVal * 2f > -4f)
                return new Vector3(0, movementBehaviour.transform.position.y + rndVal * 2f); // Player is moving DOWN
            else
                return new Vector3(0, movementBehaviour.transform.position.y);
        }
    }
}
