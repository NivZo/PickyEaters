using Godot;

public partial class AdmobProvider : Node2D
{
    public static AdmobProvider Instance { get; private set; }

    private GodotObject _admob;
    private string _currentRewardType = string.Empty;
    private bool _initialized = false;

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        _admob = GetNode("Admob");
        _admob.Call("initialize");
    }

    // TODO: Replace with event-based system
    public void ShowRewardedAd(string rewardType, bool withModal = true)
    {
        if (withModal)
        {
            ModalManager.OpenAdLoadingModal(!_initialized);
        }
        if (_initialized)
        {
            _currentRewardType = rewardType;
            _admob.Call("load_rewarded_ad");
        }
    }

    private void OnInitializationCompleted(Variant _statusData)
    {
        GD.Print("Admob initialized successfully.");
        _initialized = true;
    }

    private void OnRewardedAdLoaded(string _adId)
    {
        _admob.Call("show_rewarded_ad");
        ModalManager.CloseModal();
    }

    private void OnRewardedAdUserEarnedReward(string _adId, Variant _rewardData)
    {
        EventManager.InvokeAdRewardGranted(_currentRewardType);
    }
}
