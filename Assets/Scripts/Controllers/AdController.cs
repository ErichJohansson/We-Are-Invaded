using GoogleMobileAds.Api;
using UnityEngine;

public class AdController : MonoBehaviour
{
    private InterstitialAd gameOverAd;
    private RewardedAd dailyRewardedAd;
    private RewardedAd endGameRewardedAd;
    private int counter;
    private bool endGameAdWatched;

    public static AdController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        counter = 1;
    }

    #region Interstitial ad
    public void RequestInterstitial()
    {
        if (counter % 3 != 0)
        {
            Debug.Log("Ad will be available next time");
            return;
        }

#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#else
            string adUnitId = "unexpected_platform";
#endif

        gameOverAd = new InterstitialAd(adUnitId);
        gameOverAd.OnAdFailedToLoad += GameOverAd_OnAdFailedToLoad;
        AdRequest request = new AdRequest.Builder().Build();
        gameOverAd.LoadAd(request);
    }

    private void GameOverAd_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        Debug.Log("Failed to load end game ad");
    }

    public void DestroyInterstitialAd()
    {
        gameOverAd?.Destroy();
    }

    public void ShowInterstitialAd()
    {
        if (counter % 3 != 0 || Application.internetReachability == NetworkReachability.NotReachable || !gameOverAd.IsLoaded())
            return;
        gameOverAd?.Show();
    }

    public void IncreaseCounter()
    {
        counter++;
    }
    #endregion

    #region Daily ad
    public void RequestDailyRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#else
            string adUnitId = "unexpected_platform";
#endif

        dailyRewardedAd = new RewardedAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        dailyRewardedAd.OnUserEarnedReward += RewardedAd_OnUserEarnedReward;
        dailyRewardedAd.OnAdClosed += RewardedAd_OnAdClosed;
        dailyRewardedAd.OnAdFailedToLoad += RewardedAd_OnAdFailedToLoad;
        dailyRewardedAd.OnAdLoaded += RewardedAd_OnAdLoaded;
        dailyRewardedAd.LoadAd(request);
    }

    private void RewardedAd_OnAdLoaded(object sender, System.EventArgs e)
    {
        UIController.Instance.rewardAdButton.SetActive(true);
    }

    private void RewardedAd_OnAdFailedToLoad(object sender, AdErrorEventArgs e)
    {
        Debug.Log("Failed to load rewarded ad");
        UIController.Instance.rewardAdButton.SetActive(false);
    }

    private void RewardedAd_OnAdClosed(object sender, System.EventArgs e)
    {
        UIController.Instance.rewardAdButton.gameObject.SetActive(false);
    }

    private void RewardedAd_OnUserEarnedReward(object sender, Reward e)
    {
        GameController.Instance.AdWatchedToday = true;
        GameController.Instance.ShowDailyAd = false;
        GameController.Instance.cash += 500;
        GameController.Instance.UpdateCash();
    }

    public void ShowRewardedAd()
    {
        dailyRewardedAd?.Show();
    }
    #endregion

    #region End game ad
    public void RequestEndGameRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#else
            string adUnitId = "unexpected_platform";
#endif

        endGameRewardedAd = new RewardedAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        endGameRewardedAd.OnUserEarnedReward += EndGameRewardedAd_OnUserEarnedReward;
        endGameRewardedAd.OnAdClosed += EndGameRewardedAd_OnAdClosed;
        endGameRewardedAd.OnAdFailedToLoad += EndGameRewardedAd_OnAdFailedToLoad;
        endGameRewardedAd.OnAdLoaded += EndGameRewardedAd_OnAdLoaded;
        endGameRewardedAd.LoadAd(request);
    }

    private void EndGameRewardedAd_OnAdClosed(object sender, System.EventArgs e)
    {
        if (!endGameAdWatched) return;
        endGameAdWatched = false;
        UIController.Instance.endGameAdButton.SetActive(false);
        UIController.Instance.gameOverScreen.PartialRestart();
    }

    private void EndGameRewardedAd_OnAdLoaded(object sender, System.EventArgs e)
    {
        UIController.Instance.endGameAdButton.SetActive(true);
    }

    private void EndGameRewardedAd_OnAdFailedToLoad(object sender, AdErrorEventArgs e)
    {
        UIController.Instance.endGameAdButton.SetActive(false);
    }

    private void EndGameRewardedAd_OnUserEarnedReward(object sender, Reward e)
    {
        endGameAdWatched = true;
    }

    public void ShowEndGameRewardedAd()
    {
        endGameRewardedAd?.Show();
    }
    #endregion
}
