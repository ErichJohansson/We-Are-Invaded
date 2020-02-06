using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject bounds;

    public Collider2D frontCollider;

    private Vector3 movingTo = Vector3.zero;
    private GameController gc;


    [Header("Trail")]
    public TrailRenderer[] trails;
    public float trailLifetime;
    private Coroutine trailRoutine;

    public float Direction 
    { 
        get
        {
            return movingTo.y > gameObject.transform.position.y ? 1 : -1; 
        } 
    }
    public int CurrentHP { get; private set; }

    private void Awake()
    {
        gc = FindObjectOfType<GameController>();
        shooting = GetComponent<PlayerShooting>();
    }

    void Start()
    {
        CurrentHP = maxHP;
        gc.uc.UpdateHitPoints(this, false);
        currentSpeedBoostLength = 0;
        ActivateParallax();
    }

    #region Utility
    public void Restart()
    {
        currentSpeed = 0;
        currentSpeedBoostLength = 0;
        shooting.Restart();
        foreach (TrailRenderer trail in trails)
            trail.Clear();
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
    #endregion

    #region Movement
    public void MoveTowards(Vector3 moveTo)
    {
        movingTo = moveTo;

        Vector3 newPos = Vector3.MoveTowards(new Vector3(0, gameObject.transform.position.y), new Vector3(0, movingTo.y), currentSpeed * Time.deltaTime * turnRate);

        if (newPos.y > 1.28f || (newPos.y < -4.5f && newPos.y - gameObject.transform.position.y < 0))
            return;

        gameObject.transform.position = new Vector3(gameObject.transform.position.x, newPos.y, 9 + newPos.y / 10.00f);
    }

    public void MoveForward()
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
        gameObject.transform.position += movement;

        shooting.mainAnimator.speed = currentSpeed / maxSpeed;

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
                DealDamage(obs.hardness - hardness, gameObject.transform.position);
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
        }
    }

    public void DealDamage(int damage, Vector2 hitPosition)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            GameOver();
        }
        else
        {
            gc.uc.currentDamageEffectLength = gc.uc.damageEffectLength;
            DamagePopup.CreatePopup(damage, hitPosition);
        }
        gc.uc.UpdateHitPoints(this, true);
    }
}
