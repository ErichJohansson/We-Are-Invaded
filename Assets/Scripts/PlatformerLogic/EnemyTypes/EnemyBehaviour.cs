using System;
using System.Collections;
using Assets.Scripts.CustomEventArgs;
using UnityEngine;

public abstract class EnemyBehaviour : MonoBehaviour
{
    public BoxCollider2D viewArea;
    public float distanceThreshold;

    protected float speed;
    protected bool stop;

    protected GameObject thisGameObject;
    protected Transform thisTransform;
    protected Transform playerTransform;

    protected abstract void NextPoint();
    protected abstract IEnumerator BehaviourRoutine();
    protected abstract IEnumerator MoveTowards(Vector2 moveTowards);
    public abstract void HandleCollision(Collider2D collidedWith);

    public event EventHandler<EnemyStateEventArgs> EnemyStateChanged;

    private void Start()
    {
        thisGameObject = gameObject;
        thisTransform = thisGameObject.transform;
    }

    private void OnEnable()
    {
        if (GameController.Instance.playerObject != null)
        {
            playerTransform = GameController.Instance.playerObject.transform;
            speed = GameController.Instance.PlayerUnit.maxSpeed * 1.04f;
        }
        stop = false;
    }

    protected virtual void OnEnemyStateChanged(EnemyStateEventArgs e)
    {
        EnemyStateChanged?.Invoke(this, e);
        if (e.state) StartCoroutine(Lifetime());
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(30f);
    }

    public void ForceStop()
    {
        stop = true;
    }
}
