using System.Collections;
using System.Collections.Generic;
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

    public GameOverScreen gameOverScreen;
    public StartGameScreen startGameScreen;
    public PauseScreen pauseScreen;

    [Header("Damage Effect")]
    public Image damageEffect;
    public float damageEffectLength;
    public float currentDamageEffectLength;

    [Header("Modifier Icons")]
    public List<Image> modifierIcons;
    public Sprite empty;
    private int lastOccupiedImage;

    public static UIController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lastOccupiedImage = -1;
    }

    public int AddModifierIcon(Sprite icon)
    {
        if (lastOccupiedImage >= 4)
            return modifierIcons.Count - 1;
        lastOccupiedImage++;
        //Debug.Log(lastOccupiedImage);
        modifierIcons[lastOccupiedImage].sprite = icon;
        return lastOccupiedImage;
    }

    public void RemoveModifierIcon(Sprite iconToRemove)
    {
        for (int i = 0; i < modifierIcons.Count; i++)
        {
            if (modifierIcons[i].sprite == iconToRemove)
            {
                modifierIcons[i].sprite = empty;
                for (int j = i; j + 1 < modifierIcons.Count; j++)
                {
                    modifierIcons[j].sprite = modifierIcons[j + 1].sprite;
                    modifierIcons[j + 1].sprite = empty;
                }
                break;
            }           
        }
        lastOccupiedImage--;
    }

    public void AddScore(int score)
    {
        GameController.Instance.AddPoints(score);
        UpdateScore();
    }

    public void UpdateScore()
    {
        score.text = GameController.Instance.ScoredPoints.ToString();
    }

    public void UpdateAmmo(int currentAmmo)
    {
        ammo.text = (currentAmmo % 1000).ToString();
    }

    public void UpdateTraveledDistance()
    {
        distance.text = GameController.Instance.DistanceTraveled.ToString("F3") + " m";
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
        cash.text = GameController.Instance.cash.ToString();
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
        speedUpSlider.value = GameController.Instance.PlayerUnit.currentSpeedBoostLength / GameController.Instance.PlayerUnit.totalSpeedBoostLength;
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
