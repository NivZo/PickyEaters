using System;

public static class ShopStacksManager
{
    public const long DAILY_COOLDOWN_SECONDS = 24 * 3600; // 24 hours in seconds
    public const long HOURLY_STACK_WINDOW_SECONDS = 4 * 3600; // 4 hours in seconds
    public const int MAX_HOURLY_STACKS = 6;
    private const double Epsilon = 1e-9; // Small value for floating point comparisons

    private static DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;

    private static long GetCurrentUnixTimeSeconds() => GetUtcNow().ToUnixTimeSeconds();

    private static DateTimeOffset GetStartOfCurrentUtcDay() => GetUtcNow().Date; // Gets the date part, time is 00:00:00 UTC

    private static long GetStartOfCurrentWindow()
    {
        long currentTime = GetCurrentUnixTimeSeconds();
        long totalWindows = currentTime / HOURLY_STACK_WINDOW_SECONDS;
        return totalWindows * HOURLY_STACK_WINDOW_SECONDS;
    }

    public static bool IsDailyRewardAvailable()
    {
        long lastClaimUnixSeconds = SaveManager.ActiveSave.LastDailyFreeRewardClaimedUnixSec;
        DateTimeOffset lastClaimTime = (lastClaimUnixSeconds > 0)
            ? DateTimeOffset.FromUnixTimeSeconds(lastClaimUnixSeconds)
            : DateTimeOffset.MinValue;

        DateTimeOffset startOfToday = GetStartOfCurrentUtcDay();
        return lastClaimTime < startOfToday;
    }

    public static bool IsHourlyRewardAvailable()
    {
        return GetCurrentHourlyStacks() > 0;
    }

    public static int GetCurrentHourlyStacks()
    {
        long currentWindowStart = GetStartOfCurrentWindow();
        long currentTime = GetCurrentUnixTimeSeconds();
        long lastClaimTime = Math.Min(SaveManager.ActiveSave.LastHourlyAdRewardClaimedUnixSec, currentTime);

        if (lastClaimTime <= 0)
        {
            return MAX_HOURLY_STACKS;
        }

        long lastClaimWindowStart = (lastClaimTime / HOURLY_STACK_WINDOW_SECONDS) * HOURLY_STACK_WINDOW_SECONDS;
        long windowDifference = currentWindowStart - lastClaimWindowStart;

        if (windowDifference < 0) windowDifference = 0;

        int completeWindows = (int)(windowDifference / HOURLY_STACK_WINDOW_SECONDS);
        return Math.Min(MAX_HOURLY_STACKS, completeWindows);
    }

    public static TimeSpan GetTimeUntilDailyReward()
    {
        if (IsDailyRewardAvailable())
            return TimeSpan.Zero;

        DateTimeOffset now = GetUtcNow();
        DateTimeOffset startOfTomorrow = GetStartOfCurrentUtcDay().AddDays(1);
        TimeSpan timeLeft = startOfTomorrow - now;
        return timeLeft > TimeSpan.Zero ? timeLeft : TimeSpan.Zero;
    }

    public static TimeSpan GetTimeUntilHourlyReward()
    {
        long currentTime = GetCurrentUnixTimeSeconds();
        long currentWindowStart = GetStartOfCurrentWindow();
        long nextWindowStart = currentWindowStart + HOURLY_STACK_WINDOW_SECONDS;
        long timeLeftSeconds = nextWindowStart - currentTime;
        return TimeSpan.FromSeconds(Math.Max(0, timeLeftSeconds));
    }

    public static void ConsumeDailyReward()
    {
        if (IsDailyRewardAvailable())
        {
            SaveManager.ActiveSave.LastDailyFreeRewardClaimedUnixSec = GetStartOfCurrentUtcDay().ToUnixTimeSeconds();
            SaveManager.CommitActiveSave();
        }
    }

    public static void ConsumeHourlyReward()
    {
        int currentStacks = GetCurrentHourlyStacks();

        if (currentStacks > 0)
        {
            long currentWindowStart = GetStartOfCurrentWindow();
            int remainingStacks = currentStacks - 1;
            long newLastClaimWindowStart = currentWindowStart - (remainingStacks * HOURLY_STACK_WINDOW_SECONDS);

            SaveManager.ActiveSave.LastHourlyAdRewardClaimedUnixSec = newLastClaimWindowStart;
            SaveManager.CommitActiveSave();
        }
    }
}