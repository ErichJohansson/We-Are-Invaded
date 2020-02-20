using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.SimpleLocalization;

public class GameController : MonoBehaviour
{
    public GameObject playerObject;
    public Cinemachine.CinemachineVirtualCamera camera;

    public ParallaxController[] backgrounds;

    public int cash;

    private PlayerUnit pu;
    private bool firstLaunch;

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
    public int ScoredPoints { get; private set; }
    public float DistanceTraveled { get; private set; }
    public bool Pause { get; set; }
    public bool SomeScreenIsShown { get; set; }

    public static GameController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        SomeScreenIsShown = true;

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
        PlatformerController.Instance.RestartGame();

        // restarts player
        if (PlayerUnit != null)
            PlayerUnit.Restart();
        BossSpawner.Instance.Restart();
        Follower.Instance.Restart();

        DistanceTraveled = 0;
        UIController.Instance.UpdateTraveledDistance();
        ScoredPoints = 0;
        UIController.Instance.UpdateScore();
    }

    public void UpdateCash()
    {
        cash += ScoredPoints;
        foreach (VehicleShowcase vhcl in VehicleSelectionController.Instance.vehicles)
            vhcl.ShowVehicle();
        ScoredPoints = 0;
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
        for (int i = 0; i < VehicleSelectionController.Instance.vehicles.Length; i++)
        {
            // save other vehicle properties here if needed
            gd.tankData[i] = new TankData();
            gd.tankData[i].id = VehicleSelectionController.Instance.vehicles[i].vehicle.id;
            gd.tankData[i].purchased = VehicleSelectionController.Instance.vehicles[i].vehicle.purchased;
            gd.tankData[i].currentLevel = VehicleSelectionController.Instance.vehicles[i].vehicle.currentLevel;
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
                vhcl.ShowVehicle();

            VehicleSelectionController.Instance.SelectVehicle(initialVehicle);
            UpdateCash();
            return;
        }

        cash = gd.cash;
        firstLaunch = gd.firstLaunch;
        for (int i = 0; i < gd.tankData.Length; i++)
        {
            if (gd.tankData[i] == null)
                continue;

            foreach (VehicleShowcase vhcl in VehicleSelectionController.Instance.vehicles)
            {
                if(vhcl.vehicle.id == gd.tankData[i].id)
                {
                    vhcl.vehicle.UpdateStats(gd.tankData[i].currentLevel, gd.tankData[i].purchased);
                    vhcl.ShowVehicle();
                    break;
                }
            }
        }

        VehicleSelectionController.Instance.SelectVehicle(gd.selectedTankID);
        UpdateCash();
    }
    #endregion

    private void OnApplicationQuit()
    {
        Debug.Log("quit");
        SaveGame();
    }
}
