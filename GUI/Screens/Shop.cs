using Godot;
using System;

public partial class Shop : Node
{
    public static Shop Instance { get; private set; }
    private EaterDisplay _eaterDisplay;
    private CpuParticles2D _purchaseParticles;
    private RichTextLabel _dailyGoldRefreshCountdownLabel;
    private RichTextLabel _hourlyAdGoldRefreshCountdownLabel;
    private RichTextLabel _hourlyAdStacksLeftLabel;

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;

        _eaterDisplay = GetNode<EaterDisplay>("GUILayer/MuncherUnlock/EaterDisplay");
        _eaterDisplay.BaseScale = 3;
        _purchaseParticles = GetNode<CpuParticles2D>("GUILayer/MuncherUnlock/PurchaseParticles");
        _dailyGoldRefreshCountdownLabel = GetNode<RichTextLabel>("GUILayer/DailyFreeGold/RefreshCountdownLabel");
        _hourlyAdGoldRefreshCountdownLabel = GetNode<RichTextLabel>("GUILayer/HourlyAdGold/RefreshCountdownLabel");
        _hourlyAdStacksLeftLabel = GetNode<RichTextLabel>("GUILayer/HourlyAdGold/HourlyAdStacksLeftLabel");

        BackgroundManager.ChangeColor(NamedColor.Orange.GetColor(), lightenFactor: .7f);

        EventManager.DailyGoldButtonClicked += OnDailyGoldButtonClicked;
        EventManager.HourlyGoldButtonClicked += OnHourlyGoldButtonClicked;

        // Initially hide both labels
        _dailyGoldRefreshCountdownLabel.Visible = false;
        _hourlyAdGoldRefreshCountdownLabel.Visible = false;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.DailyGoldButtonClicked -= OnDailyGoldButtonClicked;
        EventManager.HourlyGoldButtonClicked -= OnHourlyGoldButtonClicked;
    }

    public override void _Ready()
    {
        base._Ready();

        // Daily Timer Initialization
        bool isDailyAvailable = ShopStacksManager.IsDailyRewardAvailable();
        _dailyGoldRefreshCountdownLabel.Visible = !isDailyAvailable;
        if (!isDailyAvailable)
        {
            UpdateCountdownLabel(_dailyGoldRefreshCountdownLabel, ShopStacksManager.GetTimeUntilDailyReward());
        }

        // Hourly Timer & Stacks Initialization
        int currentHourlyStacks = ShopStacksManager.GetCurrentHourlyStacks();
        bool showHourlyTimer = currentHourlyStacks < ShopStacksManager.MAX_HOURLY_STACKS;
        _hourlyAdGoldRefreshCountdownLabel.Visible = showHourlyTimer;
        if (showHourlyTimer)
        {
            UpdateCountdownLabel(_hourlyAdGoldRefreshCountdownLabel, ShopStacksManager.GetTimeUntilHourlyReward());
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // --- Daily Timer Update ---
        bool isDailyAvailable = ShopStacksManager.IsDailyRewardAvailable();
        _dailyGoldRefreshCountdownLabel.Visible = !isDailyAvailable;
        TimeSpan dailyTimeLeft = TimeSpan.Zero; // Initialize
        if (!isDailyAvailable)
        {
            dailyTimeLeft = ShopStacksManager.GetTimeUntilDailyReward();
            UpdateCountdownLabel(_dailyGoldRefreshCountdownLabel, dailyTimeLeft);
        }

        // --- Hourly Timer & Stacks Update ---
        int currentHourlyStacks = ShopStacksManager.GetCurrentHourlyStacks();
        string hourlyStacksText = $"[center][font gl=10]LEFT: {currentHourlyStacks}[/font][/center]";
        // Only update if text changed
        if (_hourlyAdStacksLeftLabel.Text != hourlyStacksText)
        {
             _hourlyAdStacksLeftLabel.Text = hourlyStacksText;
        }

        bool showHourlyTimer = currentHourlyStacks < ShopStacksManager.MAX_HOURLY_STACKS;
        _hourlyAdGoldRefreshCountdownLabel.Visible = showHourlyTimer;
        TimeSpan hourlyTimeLeft = TimeSpan.Zero; // Initialize
        if (showHourlyTimer)
        {
            hourlyTimeLeft = ShopStacksManager.GetTimeUntilHourlyReward();
            UpdateCountdownLabel(_hourlyAdGoldRefreshCountdownLabel, hourlyTimeLeft);
        }

        // --- Debug Logging (Uncomment locally if needed) ---
        // GD.Print($"Process: DailyAvailable={isDailyAvailable}, DailyLeft={dailyTimeLeft:hh\\:mm\\:ss}, HourlyStacks={currentHourlyStacks}, ShowHourlyTimer={showHourlyTimer}, HourlyLeft={hourlyTimeLeft:hh\\:mm\\:ss}");
        // GD.Print($"  Hourly Stacks Label: {_hourlyAdStacksLeftLabel.Text}");
        // GD.Print($"  Hourly Timer Label: Visible={_hourlyAdGoldRefreshCountdownLabel.Visible}, Text={_hourlyAdGoldRefreshCountdownLabel.Text}");
    }

    private void UpdateCountdownLabel(RichTextLabel label, TimeSpan timeLeft)
    {
        string newText;
        if (timeLeft <= TimeSpan.Zero)
        {
            // Ensure some text is set even when time is zero or negative
            newText = "[center][font gl=10]NEXT: 00:00:00[/font][/center]"; // Or potentially "Available"?
        }
        else
        {
            // Format positive time left
            var totalSeconds = timeLeft.TotalSeconds;
            var hours = Mathf.FloorToInt(totalSeconds / 3600);
            var minutes = Mathf.FloorToInt(totalSeconds % 3600 / 60);
            var seconds = Mathf.FloorToInt(totalSeconds % 60);
            newText = $"[center][font gl=10]NEXT: {hours:00}:{minutes:00}:{seconds:00}[/font][/center]";
        }

        // Only update the label's Text property if the content has actually changed
        if (label.Text != newText)
        {
            label.Text = newText;
            // GD.Print($"Updated Label '{label.Name}' Text: {newText}"); // Uncomment locally if needed
        }
    }

    // This method should be connected to the Daily Button's pressed signal
    private void OnDailyGoldButtonClicked()
    {
        ShopStacksManager.ConsumeDailyReward();
    }

    // This method should be connected to the Hourly Button's pressed signal
    private void OnHourlyGoldButtonClicked()
    {
        // Keep the label update for immediate feedback after the event fires
        int currentHourlyStacks = ShopStacksManager.GetCurrentHourlyStacks();
        _hourlyAdStacksLeftLabel.Text = $"[center][font gl=10]LEFT: {currentHourlyStacks}[/font][/center]";
    }
}
