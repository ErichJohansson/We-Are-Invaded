﻿using UnityEngine;
using UI;
using System;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject playerObject;
    public Cinemachine.CinemachineVirtualCamera camera;
    public Transform coinTarget;
    public ParallaxController[] backgrounds;

    public int cash;

    private PlayerUnit pu;

    [Header("New game values")]
    public int initialCash;
    public Vehicle initialVehicle;

    public PlayerUnit PlayerUnit
    {
        get { return pu; }
        set
        {
            pu = value;
            Follower.Instance.StartFollowing();
        }
    }
    public int MoneyEarned { get; private set; }
    public float DistanceTraveled { get; private set; }
    public int BarrelsExploded { get; private set; }
    public int HealthRestored { get; private set; }
    public int DamageToEnemies { get; private set; }

    public bool Pause { get; set; }
    public bool SomeScreenIsShown { get; set; }

    public static GameController Instance { get; private set; }

    public event EventHandler<EventArgs> RestartEvent;

    protected virtual void OnRestart(EventArgs e)
    {
        RestartEvent?.Invoke(this, e);
    }

    void Awake()
    {
        Instance = this;
        SomeScreenIsShown = true;

        Time.timeScale = 0;
        DamagePopup.CreatePopup(0, new Vector3(-100, 0, 0));
    }

    void Start()
    {
        LoadGame();
        Pause = true;
        gameObject.AddComponent<InputController>();
    }

    public void StartGame(bool blockLoadEffect = false)
    {
        StartCoroutine(StartRoutine(blockLoadEffect));
    }

    private IEnumerator StartRoutine(bool blockLoadEffect = false)
    {
        OnRestart(new EventArgs());
        if (!blockLoadEffect)
        {
            UIController.Instance.ChangeLoadEffectColor(new Color(0, 0, 0, 1f));
            UIController.Instance.loadEffect.gameObject.SetActive(true);
        }
        if (!PlatformerController.Instance.generated) PlatformerController.Instance.GenerateWorld();
        else PlatformerController.Instance.RegenerateLevel();
        // restarts player
        if (PlayerUnit != null) PlayerUnit.Restart();

        Follower.Instance.Restart();
        BossController.Instance.Restart();
        MoneyEarned = 0;
        HealthRestored = 0;
        BarrelsExploded = 0;
        DamageToEnemies = 0;
        DistanceTraveled = 0;
        UIController.Instance.UpdateTraveledDistance();
        UIController.Instance.UpdateScore();

        yield return new WaitForSecondsRealtime(0.5f);
        if (!blockLoadEffect)
        {
            UIController.Instance.ActivateLoadEffect(action: () => { if (!TutorialController.Instance.TutorialCompleted) TutorialController.Instance.StartTutorial(); });
        }

    }

    public void UpdateCash()
    {
        cash += MoneyEarned;
        foreach (VehicleShowcase vhcl in VehicleSelectionController.Instance.vehicles)
            vhcl.ShowVehicle();
        MoneyEarned = 0;
        UIController.Instance.UpdateCash();
    }

    public void SetPause(bool pauseState)
    {
        Pause = pauseState;
        Time.timeScale = pauseState ? 0 : 1;
    }

    #region Fire and Speed Up
    public void SpeedUp(bool state)
    {
        if (PlayerUnit != null)
            PlayerUnit.speedingUp = state;
        else
            Debug.LogError("playerUnit is NULL");
    }

    public void SwitchFiring(bool state)
    {
        if (PlayerUnit != null)
            PlayerUnit.shooting.Firing = state;
        else
            Debug.LogError("playerUnit is NULL");
    }
    #endregion

    #region Save and Load
    public void SaveGame()
    {
        GameData gd = new GameData();
        gd.cash = cash;
        gd.selectedTankID = PlayerUnit == null ? 0 : PlayerUnit.ID;
        gd.tankData = new TankData[VehicleSelectionController.Instance.vehicles.Length];
        gd.tutorialCompleted = TutorialController.Instance.TutorialCompleted;
        for (int i = 0; i < VehicleSelectionController.Instance.vehicles.Length; i++)
        {
            gd.tankData[i] = new TankData();
            gd.tankData[i].selectedColorScheme = VehicleSelectionController.Instance.vehicles[i].vehicle.selectedColorScheme;
            gd.tankData[i].id = VehicleSelectionController.Instance.vehicles[i].vehicle.id;
            gd.tankData[i].purchased = VehicleSelectionController.Instance.vehicles[i].vehicle.purchased;
            gd.tankData[i].currentLevel = VehicleSelectionController.Instance.vehicles[i].vehicle.currentLevel;
            if (VehicleSelectionController.Instance.vehicles[i].vehicle.colorSchemes != null)
                gd.tankData[i].colorShemesPurchaseState = new bool[VehicleSelectionController.Instance.vehicles[i].vehicle.colorSchemes.Length];
            for (int j = 0; j < gd.tankData[i].colorShemesPurchaseState.Length; j++)
            {
                gd.tankData[i].colorShemesPurchaseState[j] = VehicleSelectionController.Instance.vehicles[i].vehicle.colorSchemes[j].purchased;
            }
        }
        SaveManager.SaveGame(gd);
    }

    private void LoadGame()
    {
        GameData gd = SaveManager.LoadGame();
        if (gd == null)
        {
            cash = initialCash;

            initialVehicle.currentLevel = 0;
            initialVehicle.purchased = true;
            initialVehicle.UpdateStats();

            foreach (VehicleShowcase vhcl in VehicleSelectionController.Instance.vehicles)
            {
                vhcl.ShowVehicle();
            } 

            VehicleSelectionController.Instance.SelectVehicle(initialVehicle);
            UpdateCash();
            return;
        }

        cash = gd.cash;
        TutorialController.Instance.TutorialCompleted = gd.tutorialCompleted;

        for (int i = 0; i < gd.tankData.Length; i++)
        {
            if (gd.tankData[i] == null)
                continue;

            foreach (VehicleShowcase vhcl in VehicleSelectionController.Instance.vehicles)
            {
                if(vhcl.vehicle.id == gd.tankData[i].id)
                {
                    vhcl.vehicle.UpdateStats(gd.tankData[i].currentLevel, gd.tankData[i].purchased);
                    vhcl.vehicle.selectedColorScheme = gd.tankData[i].selectedColorScheme;
                    if (gd.tankData[i].colorShemesPurchaseState != null)
                    {
                        for (int j = 0; j < vhcl.vehicle.colorSchemes.Length; j++)
                            vhcl.vehicle.colorSchemes[j].purchased = gd.tankData[i].colorShemesPurchaseState[j];
                    }
                    vhcl.ShowVehicle();                    
                    break;
                }
            }
        }

        VehicleSelectionController.Instance.SelectVehicle(gd.selectedTankID);
        UpdateCash();
    }
    #endregion

    #region Various metrics
    public void AddMoney(int coins)
    {
        MoneyEarned += coins;
    }

    public void AddDistance(float val)
    {
        DistanceTraveled += Mathf.Round(val * 500f) / 1000f;
    }

    public void AddBarrel()
    {
        BarrelsExploded++;
    }

    public void AddHealthRestored(int restored)
    {
        HealthRestored += restored;
    }

    public void AddDamageToEnemies(int damage)
    {
        DamageToEnemies += damage;
    }
    #endregion

    private void OnApplicationQuit()
    {
        Debug.Log("quit");
        SaveGame();
    }
}