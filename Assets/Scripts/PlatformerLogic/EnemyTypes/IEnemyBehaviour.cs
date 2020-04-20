using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBehaviour
{
    IEnumerable BehaviourRoutine();
    Vector2 NextPoint();
    bool LookForTarget();
}
