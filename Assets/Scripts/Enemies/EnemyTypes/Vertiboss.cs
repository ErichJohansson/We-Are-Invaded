using System.Collections;
using UnityEngine;

public class Vertiboss : EnemyBehaviour
{
    public float targetDelta;
    public Sprite effectImage;
    [Range(0, 1f)] public float effectTimeScale;

    private EnemyGun shooting;
    private Coroutine movementRoutine;
    private Coroutine loadingRoutine;

    private Stance currentStance;
    private enum Stance { Roaming, BorderPos }
    private bool loading;
    private PlayerUnit player;

    private Transform topLeft, botLeft;
    private Transform dynamicEndPoint, constEndPoint;

    private void Awake()
    {
        currentStance = Stance.Roaming;
        shooting = GetComponent<EnemyGun>();
        constEndPoint = shooting.endPoint;
        dynamicEndPoint = constEndPoint;
        topLeft = GameObject.FindGameObjectWithTag("topLeft_const").transform;
        botLeft = GameObject.FindGameObjectWithTag("botLeft_const").transform;
    }

    private void FixedUpdate()
    {
        if (loading)
            return;
        ThisTransform.position += new Vector3(player.currentSpeed * Time.deltaTime, 0);
    }

    public override void HandleCollision(Collider2D collidedWith) { }

    protected override IEnumerator BehaviourRoutine()
    {
        while (ThisGameObject.activeInHierarchy || Mathf.Abs(ThisTransform.position.x - playerTransform.position.x) < 50)
        {
            if (shooting.ShotRoutine != null) 
                yield return shooting.ShotRoutine;
            else if (movementRoutine != null) 
                yield return movementRoutine;
            else
            {
                NextPoint();
                if (loading)
                    yield return loadingRoutine;
                else
                    yield return new WaitForSeconds(0.01f);
            }
        }
        ThisGameObject.SetActive(false);
    }

    protected override IEnumerator MoveTowards(Vector2 moveTowards, float spd)
    {
        shooting.unitAnimator.SetTrigger("idle");
        speed *= 0.9f;
        while (Mathf.Abs(ThisTransform.position.y - moveTowards.y) > distanceThreshold)
        {
            Vector3 v = Vector2.MoveTowards(new Vector2(0, ThisTransform.position.y), moveTowards, speed * Time.deltaTime);
            ThisTransform.position = new Vector3(ThisTransform.position.x, v.y, v.y / 10f);
            yield return new WaitForEndOfFrame();
        }
        // starts shooting when arrived to the destination
        shooting.endPoint = dynamicEndPoint;
        shooting.StartShooting();
        movementRoutine = null;
    }

    private IEnumerator LoadingRoutine()
    {
        float currentDelta = float.MaxValue;
        player = GameController.Instance.PlayerUnit;
        while (currentDelta > targetDelta)
        {
            ThisTransform.position = Vector2.MoveTowards(ThisTransform.position, playerTransform.position, speed * Time.deltaTime);
            currentDelta = ThisTransform.position.x - playerTransform.position.x;
            yield return new WaitForEndOfFrame();
        }
        loading = false;
        Debug.Log("LOADED");
        StartCoroutine(BehaviourRoutine());
    }

    protected override void NextPoint()
    {
        float nextY = 0;
        currentStance = Random.Range(0, 1f) < 0.5 ? Stance.Roaming : Stance.BorderPos;
        dynamicEndPoint = constEndPoint;
        switch (currentStance)
        {
            case Stance.Roaming:
                nextY = Random.Range(minY, maxY);
                break;
            case Stance.BorderPos:
                nextY = Random.Range(0, 1f) < 0.5 ? minY : maxY;
                dynamicEndPoint = nextY == minY ? topLeft : botLeft;
                break;
            default:
                break;
        }
        if (nextY != ThisTransform.position.y && movementRoutine == null)
            movementRoutine = StartCoroutine(MoveTowards(new Vector2(0, nextY), speed));
    }

    /// <summary>
    /// starts moving to the certain delta X to player
    /// </summary>
    public void ActivateBoss()
    {
        loading = true;
        UIController.Instance.ActivateModifierEffect(effectImage, effectTimeScale);
        loadingRoutine = StartCoroutine(LoadingRoutine());
    }

    private void OnDisable()
    {
        try
        {
            if (GetComponent<Enemy>()?.CurrentHP <= 0)
                AchievementController.Instance.UnlockAchievement(GPGSIds.achievement_helicopter_boss);
        }
        catch (System.Exception)
        {
            Debug.Log("AchievementController is null? " + (AchievementController.Instance == null) + "; <Enemy> is null? " + (GetComponent<Enemy>() == null));
        }

        BossController.Instance.Restart();
        movementRoutine = null;
        loadingRoutine = null;
        loading = false;
    }
}
