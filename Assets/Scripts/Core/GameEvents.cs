using System;

public static class GameEvents
{
    // ── Lifecycle ──────────────────────────────────────────
    public static event Action OnGameStart;
    public static event Action OnGameReset;
    public static event Action OnAllItemsFound;

    // ── Intro ──────────────────────────────────────────────
    // Fired when the player dismisses the intro title card.
    // SmartphoneCursor and AdPopupManager both wait for this
    // before activating, so the player can read the objective first.
    public static event Action OnIntroEnd;

    // ── Items ──────────────────────────────────────────────
    public static event Action<HiddenItem> OnItemFound;
    public static event Action<int, int> OnProgressUpdated;
    public static event Action<HiddenItem> OnItemHovered;
    public static event Action OnItemHoverExit;

    // ── Ads ───────────────────────────────────────────────
    public static event Action OnAdOpened;
    public static event Action OnAdClosed;

    // ── Invokers ───────────────────────────────────────────
    public static void GameStart() => OnGameStart?.Invoke();
    public static void GameReset() => OnGameReset?.Invoke();
    public static void AllItemsFound() => OnAllItemsFound?.Invoke();
    public static void IntroEnd() => OnIntroEnd?.Invoke();

    public static void ItemFound(HiddenItem i) => OnItemFound?.Invoke(i);
    public static void ProgressUpdated(int found, int total) => OnProgressUpdated?.Invoke(found, total);
    public static void ItemHovered(HiddenItem i) => OnItemHovered?.Invoke(i);
    public static void ItemHoverExit() => OnItemHoverExit?.Invoke();

    public static void AdOpened() => OnAdOpened?.Invoke();
    public static void AdClosed() => OnAdClosed?.Invoke();
}
