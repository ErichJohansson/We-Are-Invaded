﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class GameOverScreen : MonoBehaviour
{
    public UIObject panel;

    public AnimatedNumericText distanceTraveled;
    public AnimatedNumericText moneyEarned;
    public AnimatedNumericText barrelsExploded;
    public AnimatedNumericText damageToEnemies;
    public AnimatedNumericText healthRestored;

    public float loadEffectLength;

    public GameObject restartButton;

    private ObjectPooler pooler;
    private Color screenColor;

    private void Awake()
    {
        pooler = FindObjectOfType<ObjectPooler>();
        screenColor = panel.GetComponent<Image>().color;
        screenColor.a = 1;
    }

    private void SetEndGameInfo()
    {
        distanceTraveled.SetValue(GameController.Instance.DistanceTraveled);
        moneyEarned.SetValue(GameController.Instance.MoneyEarned);
        barrelsExploded.SetValue(GameController.Instance.BarrelsExploded);
        damageToEnemies.SetValue(GameController.Instance.DamageToEnemies);
        healthRestored.SetValue(GameController.Instance.HealthRestored);
    }

    public void ShowGameOverScreen()
    {
        AdController.Instance.ShowInterstitialAd();
        restartButton.SetActive(false);
        GameController.Instance.SetPause(!panel.isShown);
        SetEndGameInfo();
        UIController.Instance.ShowPanel(panel);
        UIController.Instance.ChangeLoadEffectColor(screenColor);
        UIController.Instance.ActivateLoadEffect(loadEffectLength, true, () => {
            restartButton.SetActive(true);
            AchievementController.Instance.CheckEligibleAchievements();
            AchievementController.Instance.ReportToLeaderboard((long)GameController.Instance.DistanceTraveled);
            GameController.Instance.UpdateCash();
            GameController.Instance.SaveGame();
        });
        VehicleSelectionController.Instance.UpdateColorPricetags();
    }

    public void BackToMainMenu()
    {
        GameController.Instance.mainMenuMusic.Play();
        UIController.Instance.loadEffect.gameObject.SetActive(false);
        panel.HidePanel();
        UIController.Instance.startGameScreen.panel.ShowPanel();
        if (!GameController.Instance.ReviewSuggestedToday) UIController.Instance.ActivateReviewPopup(true);
    }

    public void RestartGame()
    {
        if (UIController.Instance.Loading)
            return;

        StartCoroutine(RestartRoutine());
    }

    public void PartialRestart()
    {
        StartCoroutine(PartialRestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        pooler.Restart();
        GameController.Instance.StartGame(true);
        yield return new WaitForSecondsRealtime(0.5f);
        UIController.Instance.ChangeLoadEffectColor(new Color(0, 0, 0, 1f));
        UIController.Instance.ActivateLoadEffect(1f, action: () => { UIController.Instance.loadEffect.gameObject.SetActive(false); });
        GameController.Instance.UpdateCash();
        GameController.Instance.SetPause(!panel.isShown);
        UIController.Instance.HidePanel(panel);
    }

    private IEnumerator PartialRestartRoutine()
    {
        pooler.Restart();
        GameController.Instance.PartialRestart(true);
        yield return new WaitForSecondsRealtime(0.5f);
        UIController.Instance.ChangeLoadEffectColor(new Color(0, 0, 0, 1f));
        UIController.Instance.ActivateLoadEffect(1f, action: () => { UIController.Instance.loadEffect.gameObject.SetActive(false); });
        GameController.Instance.UpdateCash();
        GameController.Instance.SetPause(!panel.isShown);
        UIController.Instance.HidePanel(panel);
    }
}
