using GoogleMobileAds.Api;
using UnityEngine;

public class AdController : MonoBehaviour
{
    private InterstitialAd gameOverAd;
    private RewardedAd rewardedAd;
    private int counter;

    public static AdController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        counter = 1;
    }

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

        AdRequest request = new AdRequest.Builder().Build();
        gameOverAd.LoadAd(request);
    }


    public void RequestRewardAd()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#else
            string adUnitId = "unexpected_platform";
#endif

        rewardedAd = new RewardedAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.OnUserEarnedReward += RewardedAd_OnUserEarnedReward;
        rewardedAd.OnAdClosed += RewardedAd_OnAdClosed;
        rewardedAd.LoadAd(request);
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

    public void DestroyInterstitialAd()
    {
        gameOverAd?.Destroy();
    }

    public void ShowInterstitialAd()
    {
        if (counter % 3 != 0)
            return;
        gameOverAd?.Show();
    }

    public void ShowRewardedAd()
    {
        rewardedAd?.Show();
    }

    public void IncreaseCounter()
    {
        counter++;
    }
}
