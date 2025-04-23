using Godot;

public static class EventManager
{
    public delegate void GameLoadHandler();
    public static event GameLoadHandler GameLoaded;
    public static void InvokeGameLoad() => GameLoaded.Invoke();

    public delegate void ActiveSaveChangeHandler();
    public static event ActiveSaveChangeHandler ActiveSaveChanged;
    public static void InvokeActiveSaveChange() => ActiveSaveChanged.Invoke();

    public delegate void StarIncreaseHandler();
    public static event StarIncreaseHandler StarIncreased;
    public static void InvokeStarIncrease() => StarIncreased.Invoke();

    public delegate void StarsCompleteHandler();
    public static event StarsCompleteHandler StarsCompleted;
    public static void InvokeStarsComplete() => StarsCompleted.Invoke();

    public delegate void CoinsValueChangeHandler();
    public static event CoinsValueChangeHandler CoinsValueChanged;
    public static void InvokeCoinsValueChange() => CoinsValueChanged.Invoke();

    public delegate void LevelResetHandler();
    public static event LevelResetHandler LevelReset;
    public static void InvokeLevelReset() => LevelReset.Invoke();

    public delegate void FadeOutTransitionFinishHandler();
    public static event FadeOutTransitionFinishHandler FadeOutTransitionFinished;
    public static void InvokeFadeOutTransitionFinished() => FadeOutTransitionFinished.Invoke();

    public delegate void MoveSelectionStartHandler(Vector2I EaterPosId, Vector2I PossibleFoodPosId, bool IsCurrentlySelected);
    public static event MoveSelectionStartHandler MoveSelectionStarted;
    public static void InvokeMoveSelectionStarted(Vector2I eaterPosId, Vector2I possibleFoodPosId, bool isCurrentlySelected) => MoveSelectionStarted.Invoke(eaterPosId, possibleFoodPosId, isCurrentlySelected);

    public delegate void MoveSelectionCancelHandler(Vector2I EaterPosId);
    public static event MoveSelectionCancelHandler MoveSelectionCancelled;
    public static void InvokeMoveSelectionCancelled(Vector2I eaterPosId) => MoveSelectionCancelled.Invoke(eaterPosId);
    
    public delegate void MovePerformHandler(Eater eater, Food food, bool IsHint);
    public static event MovePerformHandler MovePerformed;
    public static void InvokeMovePerformed(Eater eater, Food food, bool isHint) => MovePerformed.Invoke(eater, food, isHint);
    
    public delegate void MoveUndoHandler(Vector2I EaterPosId, Vector2I FoodPosId, FoodType FoodType, bool IsLast);
    public static event MoveUndoHandler MoveUndone;
    public static void InvokeMoveUndone(Vector2I eaterPosId, Vector2I foodPosId, FoodType foodType, bool isLast) => MoveUndone.Invoke(eaterPosId, foodPosId, foodType, isLast);

    public delegate void LevelVictoryHandler();
    public static event LevelVictoryHandler LevelVictorious;
    public static void InvokeLevelVictorious() => LevelVictorious.Invoke();

    // AdMob Events
    public delegate void AdRewardRequestHandler(string rewardType, bool useModal);
    public static event AdRewardRequestHandler AdRewardRequested;
    public static void InvokeAdRewardRequested(string rewardType, bool useModal = true) => AdRewardRequested.Invoke(rewardType, useModal);

    public delegate void AdRewardCancelHanlder();
    public static event AdRewardCancelHanlder AdRewardCancelled;
    public static void InvokeAdRewardCancelled() => AdRewardCancelled.Invoke();
    
    public delegate void AdRewardGrantHandler(string rewardType);
    public static event AdRewardGrantHandler AdRewardGranted;
    public static void InvokeAdRewardGranted(string rewardType) => AdRewardGranted.Invoke(rewardType);

    public delegate void DailyGoldButtonClickHandler();
    public static event DailyGoldButtonClickHandler DailyGoldButtonClicked;
    public static void InvokeDailyGoldButtonClicked() => DailyGoldButtonClicked.Invoke();

    public delegate void HourlyGoldButtonClickHandler();
    public static event HourlyGoldButtonClickHandler HourlyGoldButtonClicked;
    public static void InvokeHourlyGoldButtonClicked() => HourlyGoldButtonClicked?.Invoke();
}