using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text score;
    public Text distance;
    public Text cash;
    public Text ammo;
    public Slider hpSlider;
    public Slider reloadSlider;
    public Slider speedUpSlider;

    private GameController gc;

    public GameOverScreen gameOverScreen;
    public StartGameScreen startGameScreen;
    public PauseScreen pauseScreen;

    [Header("Damage Effect")]
    public Image damageEffect;
    public float damageEffectLength;
    public float currentDamageEffectLength;

    void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    public void AddScore(int score)
    {
        gc.AddPoints(score);
        UpdateScore();
    }

    public void UpdateScore()
    {
        score.text = gc.ScoredPoints.ToString();
    }

    public void UpdateAmmo(int currentAmmo)
    {
        ammo.text = currentAmmo.ToString();
    }

    public void UpdateTraveledDistance()
    {
        distance.text = gc.DistanceTraveled.ToString("F3") + " m";
    }

    public void UpdateHitPoints(PlayerUnit player, bool damage)
    {
        hpSlider.value = 1f - player.CurrentHP / (float)player.maxHP;

        currentDamageEffectLength = damageEffectLength;
        if (damage)
            StartCoroutine("DamageEffect");

        Debug.Log(player.CurrentHP);
    }

    public void UpdateCash()
    {
        cash.text = gc.cash.ToString();
    }

    public void ShowPanel(UIObject menuItem)
    {
        menuItem.ShowPanel();
    }

    public void HidePanel(UIObject menuItem)
    {
        menuItem.HidePanel();
    }

    public void UpdateReloadSlider(float value)
    {
        reloadSlider.value = value;
    }

    public void UpdateSpeedUpSlider()
    {
        speedUpSlider.value = gc.PlayerUnit.currentSpeedBoostLength / gc.PlayerUnit.totalSpeedBoostLength;
    }

    public void RestartDamageEffect() {
        currentDamageEffectLength = 0;
        damageEffect.gameObject.SetActive(false);
    }

    private IEnumerator DamageEffect()
    {
        damageEffect.gameObject.SetActive(true);
        while (currentDamageEffectLength > 0)
        {
            currentDamageEffectLength -= 0.01f;
            damageEffect.color = new Color(damageEffect.color.r, damageEffect.color.g, damageEffect.color.b, currentDamageEffectLength / damageEffectLength);
            yield return new WaitForSecondsRealtime(0.005f);
        }
        currentDamageEffectLength = 0;
        damageEffect.gameObject.SetActive(false);
    }
}
