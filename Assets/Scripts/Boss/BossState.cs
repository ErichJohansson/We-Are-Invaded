using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState : MonoBehaviour
{
    public AttackPattern attackPattern;
    //public MovementPattern movementPattern;
    public MovementBehaviour movementBehaviour;

    public Formula formula;
    public bool inversed;
    public float Probability { get; private set; } // add formula usage

    public List<BossState> connectedStates;

    public void SwitchState()
    {
        if(attackPattern.Done/* && movementPattern.Done*/)
        {
            // switch state, calculate probabilities
            return;
        }
    }
}
