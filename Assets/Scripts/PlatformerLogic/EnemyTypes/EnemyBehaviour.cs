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

    private GameObject thisObject;
    private Transform thisTransform;
    protected GameObject ThisGameObject 
    { 
        get 
        { 
            if (thisObject == null)
                thisObject = gameObject;
            return thisObject;
        } 
    }
    protected Transform ThisTransform
    {
        get
        {
            if (thisTransform == null)
                thisTransform = transform;
            return thisTransform;
        }
    }
    protected Transform playerTransform;

    protected abstract void NextPoint();
    protected abstract IEnumerator BehaviourRoutine();
    protected abstract IEnumerator MoveTowards(Vector2 moveTowards);
    public abstract void HandleCollision(Collider2D collidedWith);

    [SerializeField]protected float minY = -4.2f, maxY = 4.2f;

    public event EventHandler<EnemyStateEventArgs> EnemyStateChanged;

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
