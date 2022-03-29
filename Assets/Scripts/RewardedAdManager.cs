using System;
using Unity.Services.Core;
using Unity.Services.Mediation;
using UnityEngine;

public class RewardedAdManager : MonoBehaviour
{
    public bool ClearWholeBoard;
    public bool ClearConveyor;

    IRewardedAd ad;
    string adUnitId = "Rewarded_Android";
    string gameId = "4683197";

    public async void InitServices()
    {
        try
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetGameId(gameId);
            await UnityServices.InitializeAsync(initializationOptions);

            InitializationComplete();
        }
        catch (Exception e)
        {
            InitializationFailed(e);
        }
    }

    private void Start()
    {
        InitServices();
    }

    public void SetupAd()
    {
        //Create
        ad = MediationService.Instance.CreateRewardedAd(adUnitId);

        //Subscribe to events
        ad.OnLoaded += AdLoaded;
        ad.OnFailedLoad += AdFailedLoad;

        ad.OnShowed += AdShown;
        ad.OnFailedShow += AdFailedShow;
        ad.OnClosed += AdClosed;
        ad.OnClicked += AdClicked;
        ad.OnUserRewarded += UserRewarded;

        // Impression Event
        MediationService.Instance.ImpressionEventPublisher.OnImpression += ImpressionEvent;
    }

    public void ShowAd()
    {
        if (ad.AdState == AdState.Loaded)
        {
            ad.Show();
        }
    }

    void InitializationComplete()
    {
        SetupAd();
        ad.Load();
    }

    void InitializationFailed(Exception e)
    {
        Debug.Log("Initialization Failed: " + e.Message);
    }

    void AdLoaded(object sender, EventArgs args)
    {

    }

    void AdFailedLoad(object sender, LoadErrorEventArgs args)
    {
        Debug.Log("Failed to load ad");
        Debug.Log(args.Message);
    }

    void AdShown(object sender, EventArgs args)
    {

    }

    void AdClosed(object sender, EventArgs e)
    {
        // Pre-load the next ad
        ad.Load();
        // Execute logic after an ad has been closed.
    }

    void AdClicked(object sender, EventArgs e)
    {
        // Execute logic after an ad has been clicked.
    }

    void AdFailedShow(object sender, ShowErrorEventArgs args)
    {
        Debug.Log(args.Message);
    }

    void ImpressionEvent(object sender, ImpressionEventArgs args)
    {
        var impressionData = args.ImpressionData != null ? JsonUtility.ToJson(args.ImpressionData, true) : "null";
    }

    void UserRewarded(object sender, RewardEventArgs e)
    {
        if (ClearWholeBoard)
        {
            BoardManager.Instance.ClearWholeBoard();
        }
        else if (ClearConveyor)
        {
            ConveyorController.Instance.ClearWholeConveyor();
        }
    }

}