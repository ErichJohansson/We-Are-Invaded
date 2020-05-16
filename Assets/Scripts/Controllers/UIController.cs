using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UI;

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

    [Header("Load Effect")]
    public Image loadEffect;
    public float loadEffectLength;
    [HideInInspector] public float currentLoadEffectLength;

    [Header("Modifier Icons")]
    public List<Image> modifierIcons;
    public Sprite empty;
    private int lastOccupiedImage;

    [Header("Modifier notification")]
    public float modifierEffectTime;
    public Image modifierEffect;
    public Vector3 modifierStartPos;
    public Vector3 modifierEndPos;
    public AnimationCurve modifierPositionCurve;
    public AnimationCurve modifierScaleCurve;
    private RectTransform modifierEffectTransform;

    public static UIController Instance { get; private set; }
    public bool Loading { get; private set; }
    public bool ShowingEffect { get; private set; }

    private void Awake()
    {
        Instance = this;
        modifierEffectTransform = modifierEffect.rectTransform;
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

    #region UI updates / restarts
    public void AddScore(int score)
    {
        GameController.Instance.AddMoney(score);
        UpdateScore();
    }

    public void UpdateScore()
    {
        score.text = GameController.Instance.MoneyEarned.ToString();
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

    public void RestartDamageEffect()
    {
        currentDamageEffectLength = 0;
        damageEffect.gameObject.SetActive(false);
    }
    #endregion

    public void ActivateLoadEffect(float loadLength = -1f, bool reverse = false, Action action = null)
    {
        Loading = true;
        StartCoroutine(LoadEffect(loadLength > 0 ? loadLength : loadEffectLength, reverse, action));
    }

    public Coroutine ActivateModifierEffect(Sprite img, float timeScale)
    {
        return StartCoroutine(ModifierEffect(img, timeScale));
    }

    public void ChangeLoadEffectColor(Color newColor)
    {
        loadEffect.color = newColor;
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

    private IEnumerator LoadEffect(float loadLength, bool reverse = false, Action action = null)
    {
        loadEffect.gameObject.SetActive(true);
        currentLoadEffectLength = !reverse ? loadLength : 0;
        Time.timeScale = !reverse ? 0 : 1;
        while ((currentLoadEffectLength > 0 && !reverse) || (currentLoadEffectLength < loadLength && reverse))
        {
            currentLoadEffectLength += !reverse ? -0.01f : 0.01f;
            loadEffect.color = new Color(loadEffect.color.r, loadEffect.color.g, loadEffect.color.b, currentLoadEffectLength / loadLength);
            yield return new WaitForSecondsRealtime(0.005f);
        }
        currentLoadEffectLength = 0;
        loadEffect.gameObject.SetActive(reverse);
        if (reverse)
            yield return new WaitForSeconds(0.25f);
        Time.timeScale = !reverse ? 1 : 0;
        Loading = false;
        if (action != null)
            action.Invoke();
    }

    private IEnumerator ModifierEffect(Sprite img, float timeScale)
    {
        float t = 0;
        Time.timeScale = timeScale;
        modifierEffect.sprite = img;
        ShowingEffect = true;
        while (t < modifierEffectTime)
        {
            t += Time.deltaTime;
            modifierEffectTransform.anchoredPosition = Vector3.Lerp(modifierStartPos, modifierEndPos, modifierPositionCurve.Evaluate(t / modifierEffectTime));
            modifierEffectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, modifierScaleCurve.Evaluate(t / modifierEffectTime));
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1f;
        ShowingEffect = false;
    }
}