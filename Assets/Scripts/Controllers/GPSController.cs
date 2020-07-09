using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using UnityEngine;

public class GPSController : MonoBehaviour
{
    public static GPSController Instance { get; private set; }
    [SerializeField] private string cloudSaveName;
    [SerializeField] private DataSource dataSource;
    [SerializeField] private ConflictResolutionStrategy conflictStrategy;
    public PlayGamesPlatform platform;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        PlayGamesClientConfiguration.Builder builder = new PlayGamesClientConfiguration.Builder();
        builder.EnableSavedGames();

        PlayGamesPlatform.InitializeInstance(builder.Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        platform = PlayGamesPlatform.Activate();
    }

    // Auth
    public void SignIn(Action successCallback = null, Action errorCallback = null)
    {
        try
        {
            Social.localUser.Authenticate((bool success) => { 
                if (success)
                {
                    successCallback?.Invoke();
                    Debug.Log("login successful");
                }
            });

        }
        catch (Exception e)
        {
            Debug.Log(e);
            errorCallback?.Invoke();
        }
    }

    public void SignOut()
    {
        if (Social.localUser.authenticated) PlayGamesPlatform.Instance.SignOut();
    }

    // Achievements

    // Leaderboard

    // Cloud saves
    public void OpenCloudSave(Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {
        platform.SavedGame.OpenWithAutomaticConflictResolution("gamedata.bin", dataSource, conflictStrategy, callback);
        Debug.Log("Cloud save opened");
    }
}
