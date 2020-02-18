using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerUnit : MonoBehaviour
{
    public PlayerShooting shooting;
    public int ID;

    public int maxHP;
    public int hardness;
    public float maxSpeed;
    public float currentSpeed;
    public float speedUpRate;
    public float turnRate;

    public float totalSpeedBoostLength;
    public float currentSpeedBoostLength;
    public bool speedingUp;

    public Collider2D frontCollider;
    public Collider2D sideCollider;

    [Header("Trail")]
    public TrailRenderer[] trails;
    public float trailLifetime;
    private Coroutine trailRoutine;

    private Vector3 movingTo = Vector3.zero;
    private GameController gc;
    private SpriteRenderer sr;

    private InfiniteAmmo infiniteAmmo;
    public InfiniteAmmo InfiniteAmmo { get { return infiniteAmmo; } set { infiniteAmmo = value; } }

    private IncreasedDamage increasedDamage;
    public IncreasedDamage IncreasedDamage { get { return increasedDamage; } set { increasedDamage = value; } }

    private SpeedBoost speedBoost;
    public SpeedBoost SpeedBoost { get { return speedBoost; } set { speedBoost = value; } }

    public float Direction 
    { 
        get
        {
            return movingTo.y > gameObject.transform.position.y ? 1 : -1; 
        } 
    }
    public int CurrentHP { get; private set; }
    public bool IsFastTraveling { get; private set; }

    private Vector3 start;
    private Vector3 finish;
    private float t;
    private float fastTravelingTime;
    private Transform playerTansform;

    private void Awake()
    {
        speedBoost = null;
        infiniteAmmo = null;
        increasedDamage = null;

        fastTravelingTime = 0f;
        t = 0f;

        sr = GetComponent<SpriteRenderer>();
        gc = FindObjectOfType<GameController>();
        shooting = GetComponent<PlayerShooting>();
        playerTansform = transform;
    }

    void Start()
    {
        CurrentHP = maxHP;
        gc.uc.UpdateHitPoints(this, false);
        currentSpeedBoostLength = 0;
        ActivateParallax();
    }

    void Update()
    {
        if (gc.Pause)
            return;
        if (IsFastTraveling)
        {
            t += Time.deltaTime / fastTravelingTime;
            playerTansform.position = Vector3.Lerp(start, finish, t);
            if (Mathf.Abs(playerTansform.position.x - finish.x) < 1)
            {
                gc.AddDistance(finish.x - start.x);
                IsFastTraveling = false;
                SpeedBoost.Deactivate();
            }
            return;
        }
        MoveForward();
    }

    #region Utility
    public void Restart()
    {
        if(infiniteAmmo != null)
            infiniteAmmo.Deactivate();
        if (increasedDamage != null)
            increasedDamage.Deactivate();
        if (speedBoost != null)
            speedBoost.Deactivate(true);

        currentSpeed = 0;
        currentSpeedBoostLength = 0;
        shooting.Restart();
        foreach (TrailRenderer trail in trails)
        {
            trail.Clear();
            trail.emitting = false;
        }
        StopAllCoroutines();
        ActivateParallax();
        gc.uc.RestartDamageEffect();
        CurrentHP = maxHP;
        gc.uc.UpdateHitPoints(this, false);
    }

    private void GameOver()
    {
        gameObject.SetActive(false);
        gc.uc.RestartDamageEffect();
        gc.uc.gameOverScreen.ShowGameOverScreen();
        foreach (TrailRenderer trail in trails)
        {
            trail.Clear();
            trail.emitting = false;
        }
    }

    public void ApplyStats(Vehicle vehicle)
    {
        GetComponent<Animator>().runtimeAnimatorController = vehicle.animatorControllers[vehicle.selectedColorScheme];
        maxSpeed = vehicle.speed;
        maxHP = vehicle.health;
        turnRate = vehicle.turning;
        shooting.reloadTime = vehicle.reloadTime;
        shooting.baseDamage = vehicle.damage;
        CurrentHP = maxHP;
        gc.uc.UpdateHitPoints(this, false);
    }
    #endregion

    #region Visuals
    private void ActivateParallax()
    {
        //bounds.transform.position = gameObject.transform.position;
        for (int i = 0; i < gc.backgrounds.Length; i++)
        {
            gc.backgrounds[i].player = gameObject;
            gc.backgrounds[i].Activate();
        }
    }

    private IEnumerator TrailLifetime()
    {
        float t = 0;
        foreach (TrailRenderer trail in trails)
            trail.emitting = true;
        while (t < trailLifetime)
        {
            t += Time.deltaTime * currentSpeed;
            yield return new WaitForEndOfFrame();
        }
        trailLifetime = 0;
        foreach (TrailRenderer trail in trails)
            trail.emitting = false;
        trailRoutine = null;
    }

    private IEnumerator Invincible()
    {
        IsFastTraveling = false;
        float t = 0;
        float waitTime = 0.25f;
        while(t < 4f)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (t / waitTime) % 2);
            t += waitTime;
            yield return new WaitForSeconds(waitTime);
        }
        MakeVulnerable(false);
    }
    #endregion

    #region Movement
    public void MoveTowards(Vector3 moveTo)
    {
        movingTo = moveTo;

        Vector3 newPos = Vector3.MoveTowards(new Vector3(0, playerTansform.position.y), new Vector3(0, movingTo.y), currentSpeed * Time.deltaTime * turnRate);

        if (newPos.y > 1.28f || (newPos.y < -4.5f && newPos.y - playerTansform.position.y < 0))
            return;

        playerTansform.position = new Vector3(playerTansform.position.x, newPos.y, 9 + newPos.y / 10.00f);
    }

    private void MoveForward()
    {
        if (speedingUp)
        {
            if (currentSpeedBoostLength > 0)
                SpeedUp();
            else
                speedingUp = false;
        }
        else if (currentSpeed < maxSpeed)
            currentSpeed += Time.deltaTime * speedUpRate / 5;
        else if (!speedingUp)
            currentSpeed -= Time.deltaTime * speedUpRate;

        if (!speedingUp && currentSpeedBoostLength < totalSpeedBoostLength)
        {
            currentSpeedBoostLength += Time.deltaTime;
            gc.uc.UpdateSpeedUpSlider();
        }

        Vector3 movement = new Vector3(currentSpeed * Time.deltaTime, 0);
        playerTansform.position += movement;
        float animSpeed = currentSpeed / maxSpeed;
        shooting.mainAnimator.speed = animSpeed < 1 ? 1 : animSpeed;

        if(Time.timeScale != 0)
        {
            gc.AddDistance(Time.deltaTime * gc.PlayerUnit.currentSpeed);
            gc.uc.UpdateTraveledDistance();
        }
    }

    public void SpeedUp()
    {
        if (currentSpeed < 2 * maxSpeed)
        {
            currentSpeed += Time.deltaTime * speedUpRate;
        }
        currentSpeedBoostLength -= 2 * Time.deltaTime;

        gc.uc.UpdateSpeedUpSlider();
    }

    private IEnumerator FastTraveling(float distanceX, float fastTravelingTime)
    {
        float t = 0;

        while(t < fastTravelingTime)
        {
            t += Time.deltaTime / fastTravelingTime;
            gameObject.transform.position = Vector3.Lerp(start, finish, t);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D col)
    {
        Obstacle obs = null;
        if (col.gameObject.transform.gameObject.TryGetComponent(out obs))
        {
            if (obs.hierarchyObject)
                return;

            currentSpeed -= speedingUp ? 0 : currentSpeed - obs.slowAmount >= 0 ? obs.slowAmount : currentSpeed;
            if (obs.hardness > hardness)
                DealDamage(obs.hardness - hardness, gameObject.transform.position, false);
            float diePosZ = gameObject.transform.position.z;
            if(obs.hardness <= hardness)
            {
                obs.Die(diePosZ);
                trailLifetime += obs.bloodTrailLength;
                EnemyShooting es = obs.GetComponent<EnemyShooting>();
                if (es != null)
                    es.Stop();
                if(trailRoutine == null)
                    trailRoutine = StartCoroutine("TrailLifetime");
            }
            return;
        }
    }

    public void DealDamage(int damage, Vector2 hitPosition, bool critical)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            GameOver();
        }
        else
        {
            gc.uc.currentDamageEffectLength = gc.uc.damageEffectLength;
            DamagePopup.CreatePopup(damage, hitPosition, critical);
        }
        gc.uc.UpdateHitPoints(this, true);
    }

    public void MakeInvincible(bool startBlinking)
    {
        frontCollider.enabled = false;
        sideCollider.enabled = false;
        if (startBlinking)
        {
            currentSpeed = maxSpeed;
            StartCoroutine("Invincible");
        }
    }

    public void MakeVulnerable(bool stopBlinking)
    {
        if(stopBlinking)
            StopCoroutine("Invincible");
        frontCollider.enabled = true;
        sideCollider.enabled = true;
    }

    public void Repair(int hp)
    {
        CurrentHP = CurrentHP + hp >= maxHP ? maxHP : CurrentHP + hp;
        gc.uc.UpdateHitPoints(this, false);
    }

    public void FastTravel(float time)
    {
        fastTravelingTime = time;
        t = 0f;

        IsFastTraveling = true;
        start = gameObject.transform.position;
        finish = new Vector3(start.x + 150, start.y, start.z);
    }
}
