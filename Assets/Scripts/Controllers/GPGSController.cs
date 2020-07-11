using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using UnityEngine;

public class GPGSController : MonoBehaviour
{
    public static GPGSController Instance { get; private set; }
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
                else
                {
                    errorCallback?.Invoke();
                    Debug.Log("login failed");
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

    // Cloud saves
    public void OpenCloudSave(Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {
        try
        {
            platform.SavedGame.OpenWithAutomaticConflictResolution("weareinvaded_save", dataSource, conflictStrategy, callback);
            Debug.Log("Cloud save opened");
        }
        catch (Exception)
        {
            Debug.Log("Can't open cloud save");
        }
    }
}
