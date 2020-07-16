using UnityEngine;
using UI;
using System;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject playerObject;
    public Cinemachine.CinemachineVirtualCamera camera;
    public ParallaxController[] backgrounds;
    public int cash;

    [Header("Tutorial")]
    public TutorialController tutorial;
    public TutorialController cutScene;

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
    public int ModifiersCollected { get; private set; }
    public DateTime LastLaunch { get; private set; }
    public bool AdWatchedToday { get; set; }
    public bool ReviewSuggestedToday { get; set; }
    public bool ShowDailyAd { get; set; }

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

        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        Time.timeScale = 0;
        DamagePopup.CreatePopup(0, new Vector3(-100, 0, 0));
    }

    void Start()
    {
        ProcessLoadData();
        if (!cutScene.TutorialCompleted) cutScene.StartTutorial();

        GPGSController.Instance.SignIn();

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
        AdController.Instance.IncreaseCounter();
        AdController.Instance.RequestInterstitial();
        if (!blockLoadEffect)
        {
            UIController.Instance.ChangeLoadEffectColor(new Color(0, 0, 0, 1f));
            UIController.Instance.loadEffect.gameObject.SetActive(true);
        }
        if (!LevelController.Instance.generated) LevelController.Instance.GenerateWorld();
        else LevelController.Instance.RegenerateLevel();
        // restarts player
        if (PlayerUnit != null) PlayerUnit.Restart();

        Follower.Instance.Restart();
        BossController.Instance.Restart();
        MoneyEarned = 0;
        HealthRestored = 0;
        BarrelsExploded = 0;
        DamageToEnemies = 0;
        DistanceTraveled = 0;
        ModifiersCollected = 0;
        UIController.Instance.UpdateTraveledDistance();
        UIController.Instance.UpdateScore();

        yield return new WaitForSecondsRealtime(0.5f);
        if (!blockLoadEffect)
        {
            UIController.Instance.ActivateLoadEffect(1f, action: () => { if (!tutorial.TutorialCompleted) tutorial.StartTutorial(); });
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
        gd.lastLaunch = DateTime.Now;
        gd.adWatchedToday = AdWatchedToday;
        gd.reviewSuggestedToday = ReviewSuggestedToday; 
        gd.cash = cash;
        gd.selectedTankID = PlayerUnit == null ? 0 : PlayerUnit.ID;
        gd.tankData = new TankData[VehicleSelectionController.Instance.vehicles.Length];
        gd.tutorialCompleted = tutorial.TutorialCompleted;
        gd.cutSceneCompleted = cutScene.TutorialCompleted;
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
        SaveController.Instance.SaveGame(gd);
    }

    private void ProcessLoadData()
    {
        Debug.Log("Load started");
        GameData gd = SaveController.Instance.LoadGame();
        FindObjectOfType<ScrollSnap>()?.gameObject.SetActive(true);

        if (gd == null)
        {
            Debug.Log("creating default save");
            cash = initialCash;

            initialVehicle.currentLevel = 0;
            initialVehicle.purchased = true;
            initialVehicle.UpdateStats();
            LastLaunch = DateTime.Now;
            AdWatchedToday = false;
            ShowDailyAd = true;

            foreach (VehicleShowcase vhcl in VehicleSelectionController.Instance.vehicles)
                vhcl.ShowVehicle();

            UIController.Instance.rewardAdButton.SetActive(true);
            AdController.Instance.RequestRewardAd();
            VehicleSelectionController.Instance.SelectVehicle(initialVehicle);
            UpdateCash();
            SaveGame();
            Debug.Log("default save created");
            return;
        }

        Debug.Log("Loading data from existing save");

        cash = gd.cash;

        if (!gd.adWatchedToday)
        {
            AdWatchedToday = false;
            ShowDailyAd = true;
        }
        else if ((gd.lastLaunch.DayOfYear - DateTime.Now.DayOfYear) != 0)
        {
            AdWatchedToday = false;
            ShowDailyAd = true;
            ReviewSuggestedToday = false;
        }
        else
        {
            AdWatchedToday = gd.adWatchedToday;
            ReviewSuggestedToday = gd.reviewSuggestedToday;
            ShowDailyAd = false;
        }

        VehicleSelectionController.Instance.SelectVehicle(gd.selectedTankID);
        tutorial.TutorialCompleted = gd.tutorialCompleted;
        cutScene.TutorialCompleted = gd.cutSceneCompleted;
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
                        {
                            try
                            {
                                vhcl.vehicle.colorSchemes[j].purchased = gd.tankData[i].colorShemesPurchaseState[j];
                            }
                            catch (IndexOutOfRangeException)
                            {
                                vhcl.vehicle.colorSchemes[j].purchased = false;
                            }
                        }
                    }
                    vhcl.ShowVehicle();                    
                    break;
                }
            }
        }

        if (ShowDailyAd)
        {
            UIController.Instance.rewardAdButton.SetActive(true);
            AdController.Instance.RequestRewardAd();
        }

        UpdateCash();
        Debug.Log("Load finished");
    }
    #endregion

    #region Various metrics
    public void AddMoney(int coins)
    {
        MoneyEarned += coins;
    }

    public void AddModifier()
    {
        ModifiersCollected++;
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
        SaveGame();
        Debug.Log("quitting");
    }
}