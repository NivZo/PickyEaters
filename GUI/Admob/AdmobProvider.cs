using System;
using Godot;

public partial class AdmobProvider : Node2D
{
    private GodotObject _admob;
    private string _currentRewardType = string.Empty;
    private bool _isRequestActive = false;
    private bool _isLoaded = false;
    private bool _initialized = false;
    private bool _isRewardGranted = false;

    public override void _Ready()
    {
        base._Ready();
        _admob = GetNode("Admob");
        _admob.Call("initialize");

        EventManager.AdRewardRequested += OnAdRewardRequested;
        EventManager.AdRewardCancelled += OnAdRewardCancelled;
    }

    private void OnAdRewardCancelled()
    {
        _isRequestActive = false;
        _isRewardGranted = false;
        ModalManager.CloseModal();
        _admob.Call("load_rewarded_ad");
    }


    public void OnAdRewardRequested(string rewardType, bool withModal = true)
    {
        _isRequestActive = true;
        _currentRewardType = rewardType;

        if (withModal)
        {
            ModalManager.OpenAdLoadingModal(!_initialized);
        }

        if (_initialized)
        {
            if (_isLoaded)
            {
                _admob.Call("show_rewarded_ad");
            }
            else
            {
                _admob.Call("load_rewarded_ad");
            }
        }
    }

    private void OnAdmobFailedToLoad(string _error)
    {
        if (_isRequestActive)
        {
            _isRequestActive = false;
            _isRewardGranted = false;
            _isLoaded = false;
            if (ModalManager.CurrentOpenModal == ModalManager.ModalType.AdLoadingModal)
            {
                ModalManager.CloseModal(true);
                ModalManager.OpenAdLoadingModal(true);
            }

            _admob.Call("load_rewarded_ad");
        }
    }

    private void OnAdmobFailedToShow(string _adId, string _error) => OnAdmobFailedToLoad(_error);

    private void OnAdmobInitializationCompleted(Variant _statusData)
    {
        _initialized = true;
        _admob.Call("load_rewarded_ad");
    }

    private void OnAdmobRewardedAdLoaded(string _adId)
    {
        _isLoaded = true;
        if (_isRequestActive)
        {
            _admob.Call("show_rewarded_ad");
        }
    }

    private void OnAdmobRewardedAdShown(string _adId)
    {
        _isLoaded = false;
        _isRequestActive = false;
        if (ModalManager.CurrentOpenModal == ModalManager.ModalType.AdLoadingModal)
        {
            ModalManager.CloseModal(true);
        }
        
        _admob.Call("load_rewarded_ad");
    }

    private void OnAdmobRewardedAdUserEarnedReward(string _adId, Variant _rewardData)
    {
        _isRewardGranted = true;
    }
    
    private void OnAdmobRewardedAdDismissedFullScreen(string _adId)
    {
        if (_isRewardGranted)
        {
            EventManager.InvokeAdRewardGranted(_currentRewardType);
            _isRewardGranted = false;
        }
    }
}
