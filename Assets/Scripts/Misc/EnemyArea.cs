using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyArea : MonoBehaviour
{
    public BoxCollider2D spawnArea;
    public List<GameObject> allowedEnemies;
    public int maxObjectsInArea;
    public int triesPerObject;
}
