using UnityEngine;
using System;
using Assets.SimpleLocalization;

public class GameController : MonoBehaviour
{
    public GameObject playerObject;
    public Cinemachine.CinemachineVirtualCamera camera;

    public PlatformerController pc;
    public UIController uc;
    public VehicleSelectionController vehicleSelection;
    public BossSpawner bossSpawner;
    public Follower follower;

    public ParallaxController[] backgrounds;

    private PlayerUnit pu;
    public PlayerUnit PlayerUnit {
        get { return pu; } 
        set
        {
            pu = value;
            if(follower != null)
                follower.StartFollowing();
        } 
    }

    public bool someScreenIsShown;

    public int cash;

    [Header("New game values")]
    public int initialCash;
    public Vehicle initialVehicle;

    public int ScoredPoints { get; private set; }
    public float DistanceTraveled { get; private set; }
    public bool Pause { get; set; }

    private bool firstLaunch;

    void Awake()
    {
        someScreenIsShown = true;

        LocalizationManager.Read();
        switch (Application.systemLanguage)
        {
            case SystemLanguage.German:
                LocalizationManager.Language = "German";
                break;
            case SystemLanguage.Russian:
                LocalizationManager.Language = "Russian";
                break;
            default:
                LocalizationManager.Language = "English";
                break;
        }

        Time.timeScale = 0;
        DamagePopup.CreatePopup(0, new Vector3(-100, 0, 0));
        ExplosionEffect.CreateEffect(new Vector3(-100, 0, 0), "explosion", gameObject.transform);
    }

    void Start()
    {
        LoadGame();
        Pause = true;
        gameObject.AddComponent<InputController>();
    }

    public void AddPoints(int pts)
    {
        ScoredPoints += pts;
    }

    public void AddDistance(float val) 
    {
        DistanceTraveled += Mathf.Round(val * 500f) / 1000f;
    }

    public void StartGame()
    {
        // restarts game, create new level
        pc.RestartGame();

        // restarts player
        if (PlayerUnit != null)
            PlayerUnit.Restart();
        if (bossSpawner != null)
            bossSpawner.Restart();
        if(follower != null)
            follower.Restart();

        foreach (RemainsSpawner rs in FindObjectsOfType<RemainsSpawner>())
            rs.Restart();

        DistanceTraveled = 0;
        uc.UpdateTraveledDistance();
        ScoredPoints = 0;
        uc.UpdateScore();
    }

    public void UpdateCash()
    {
        cash += ScoredPoints;
        ScoredPoints = 0;
        uc.UpdateCash();
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
        gd.tankData = new TankData[vehicleSelection.vehicles.Length];
        for (int i = 0; i < vehicleSelection.vehicles.Length; i++)
        {
            // save other vehicle properties here if needed
            gd.tankData[i] = new TankData();
            gd.tankData[i].id = vehicleSelection.vehicles[i].id;
            gd.tankData[i].purchased = vehicleSelection.vehicles[i].purchased;
        }
        SaveManager.SaveGame(gd);
    }

    private void LoadGame()
    {
        GameData gd = SaveManager.LoadGame();
        if (gd == null)
        {
            cash = initialCash;
            vehicleSelection.SelectVehicle(initialVehicle);
            UpdateCash();
            return;
        }

        cash = gd.cash;
        firstLaunch = gd.firstLaunch;
        for (int i = 0; i < gd.tankData.Length; i++)
        {
            for (int j = 0; j < vehicleSelection.vehicles.Length; j++)
            {
                if(vehicleSelection.vehicles[j].id == gd.tankData[i].id)
                {
                    vehicleSelection.vehicles[j].purchased = gd.tankData[i].purchased;
                    // set other vehicle properties here if needed
                    break;
                }
            }
        }
        vehicleSelection.SelectVehicle(gd.selectedTankID);
        UpdateCash();
    }
    #endregion

    private void OnApplicationQuit()
    {
        Debug.Log("quit");
        SaveGame();
    }
}
