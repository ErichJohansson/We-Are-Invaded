using System.Collections;
using UI;
using UnityEngine;

[System.Serializable]
public class PlayerUnit : DamageReciever
{
    public PlayerShooting shooting;
    public int ID;

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
    public bool IsFastTraveling { get; private set; }

    private Vector3 start;
    private Vector3 finish;
    private float t;
    private float fastTravelingTime;
    private Animator animator;
    private bool turningRight;
    private bool turningLeft;

    private GameObject thisGameObject;
    private Transform thisTransform;

    private void Awake()
    {
        thisGameObject = gameObject;
        thisTransform = transform;

        animator = GetComponent<Animator>();
        speedBoost = null;
        infiniteAmmo = null;
        increasedDamage = null;

        fastTravelingTime = 0f;
        t = 0f;

        sr = GetComponent<SpriteRenderer>();
        shooting = GetComponent<PlayerShooting>();
    }

    void Start()
    {
        CurrentHP = maxHP;
        UIController.Instance.UpdateHitPoints(this, false);
        currentSpeedBoostLength = 0;
        ActivateParallax();
    }

    void Update()
    {
        if (GameController.Instance.Pause)
            return;
        if (IsFastTraveling)
        {
            t += Time.deltaTime / fastTravelingTime;
            thisTransform.position = Vector3.Lerp(start, finish, t);
            if (Mathf.Abs(thisTransform.position.x - finish.x) < 1)
            {
                GameController.Instance.AddDistance(finish.x - start.x);
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
            speedBoost.ForceDeactivation();

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
        UIController.Instance.RestartDamageEffect();
        CurrentHP = maxHP;
        UIController.Instance.UpdateHitPoints(this, false);
    }

    private void GameOver()
    {
        UIController.Instance.RestartDamageEffect();
        UIController.Instance.gameOverScreen.ShowGameOverScreen();
        foreach (TrailRenderer trail in trails)
        {
            trail.Clear();
            trail.emitting = false;
        }
    }

    public void ApplyStats(Vehicle vehicle)
    {
        animator.runtimeAnimatorController = vehicle.colorSchemes[vehicle.selectedColorScheme].animatorController;
        maxSpeed = vehicle.speed;
        maxHP = vehicle.health;
        turnRate = vehicle.turning;
        shooting.reloadTime = vehicle.reloadTime;
        shooting.baseDamage = vehicle.damage;
        CurrentHP = maxHP;
        UIController.Instance.UpdateHitPoints(this, false);
    }
    #endregion

    #region Visuals
    private void ActivateParallax()
    {
        for (int i = 0; i < GameController.Instance.backgrounds.Length; i++)
        {
            GameController.Instance.backgrounds[i].Restart();
            GameController.Instance.backgrounds[i].player = thisGameObject;
            GameController.Instance.backgrounds[i].Setup();
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
        while(t < 2f)
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

        if(thisTransform.position.y > movingTo.y)
        {
            turningRight = true;
            turningLeft = false;
        }
        else if (thisTransform.position.y < movingTo.y)
        {
            turningRight = false;
            turningLeft = true;
        }
        else 
        {
            StopTurning();
        }
        animator.SetBool("turningRight", turningRight);
        animator.SetBool("turningLeft", turningLeft);

        Vector3 newPos = Vector3.MoveTowards(new Vector3(0, thisTransform.position.y), new Vector3(0, movingTo.y), currentSpeed * Time.deltaTime * turnRate);

        if (newPos.y > 1.28f || (newPos.y < -4.5f && newPos.y - thisTransform.position.y < 0))
            return;

        thisTransform.position = new Vector3(thisTransform.position.x, newPos.y, 9 + newPos.y / 10.00f);
    }

    public void StopTurning()
    {
        turningRight = false;
        turningLeft = false;
        animator.SetBool("turningRight", turningRight);
        animator.SetBool("turningLeft", turningLeft);
        animator.SetTrigger("drivingStraight");
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
            UIController.Instance.UpdateSpeedUpSlider();
        }

        Vector3 movement = new Vector3(currentSpeed * Time.deltaTime, 0);
        thisTransform.position += movement;

        if(Time.timeScale != 0)
        {
            GameController.Instance.AddDistance(Time.deltaTime * currentSpeed);
            UIController.Instance.UpdateTraveledDistance();
        }
    }

    public void SpeedUp()
    {
        if (currentSpeed < 2 * maxSpeed)
        {
            currentSpeed += Time.deltaTime * speedUpRate;
        }
        currentSpeedBoostLength -= 2 * Time.deltaTime;

        UIController.Instance.UpdateSpeedUpSlider();
    }

    private IEnumerator FastTraveling(float fastTravelingTime)
    {
        float t = 0;

        while(t < fastTravelingTime)
        {
            t += Time.deltaTime / fastTravelingTime;
            thisTransform.position = Vector3.Lerp(start, finish, t);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D col)
    {
        Obstacle obs = col.gameObject.GetComponent<Obstacle>();
        if (obs != null)
        {
            currentSpeed -= speedingUp ? 0 : currentSpeed - obs.slowAmount >= 0 ? obs.slowAmount : currentSpeed;
            if (obs.hardness > hardness)
                ReceiveDamage(obs.hardness - hardness, thisTransform.position, false);

            if(obs.hardness < 9999)
            {
                if (trailRoutine == null)
                    trailRoutine = StartCoroutine("TrailLifetime");
                obs.OnDeath(new System.EventArgs());
            }

            Enemy e = col.gameObject.GetComponent<Enemy>();
            if (e != null) e.OnDeath(new System.EventArgs());
        }
    }

    public void ReceiveDamage(int damage, Vector2 pos, bool critical = false)
    {
        DamagePopup.CreatePopup(damage, pos, critical);
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            GameOver();
        }
        else
        {
            UIController.Instance.currentDamageEffectLength = UIController.Instance.damageEffectLength;
        }
        UIController.Instance.UpdateHitPoints(this, true);
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
        UIController.Instance.UpdateHitPoints(this, false);
    }

    public void FastTravel(float time)
    {
        fastTravelingTime = time;
        t = 0f;

        IsFastTraveling = true;
        start = thisTransform.position;
        finish = new Vector3(start.x + 150, start.y, start.z);
    }
}
