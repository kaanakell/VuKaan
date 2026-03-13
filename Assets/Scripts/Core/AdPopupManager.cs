using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdPopupManager : MonoBehaviour
{
    [System.Serializable]
    public struct AdEntry
    {
        public Sprite image;
        public AudioClip sound;   // leave null to use no sound for this specific ad
    }

    [Header("References")]
    public GameObject adPopupPrefab;
    public RectTransform phoneCanvas;

    [Header("Ad Content")]
    [Tooltip("Each entry pairs an ad image with its own sound clip.")]
    public List<AdEntry> ads = new();

    [Header("Timing")]
    public float minInterval = 15f;
    public float maxInterval = 30f;
    public float initialDelay = 8f;
    public float hoverGracePeriod = 6f;

    // ── State ──────────────────────────────────────────────
    private bool _gameReady = false;
    private bool _adOpen = false;
    private bool _isHovering = false;
    private float _hoverStartTime = -999f;
    private Coroutine _spawnRoutine = null;

    // ── Unity ──────────────────────────────────────────────
    private void OnEnable()
    {
        GameEvents.OnIntroEnd += HandleIntroEnd;
        GameEvents.OnGameReset += HandleGameReset;
        GameEvents.OnAllItemsFound += HandleGameOver;
        GameEvents.OnAdOpened += HandleAdOpened;
        GameEvents.OnAdClosed += HandleAdClosed;
        GameEvents.OnItemHovered += HandleItemHovered;
        GameEvents.OnItemHoverExit += HandleHoverExit;
    }

    private void OnDisable()
    {
        GameEvents.OnIntroEnd -= HandleIntroEnd;
        GameEvents.OnGameReset -= HandleGameReset;
        GameEvents.OnAllItemsFound -= HandleGameOver;
        GameEvents.OnAdOpened -= HandleAdOpened;
        GameEvents.OnAdClosed -= HandleAdClosed;
        GameEvents.OnItemHovered -= HandleItemHovered;
        GameEvents.OnItemHoverExit -= HandleHoverExit;
    }

    // ── Handlers ───────────────────────────────────────────
    private void HandleIntroEnd()
    {
        _gameReady = true;
        if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);
        _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void HandleGameReset()
    {
        _gameReady = false;
        _adOpen = false;
        _isHovering = false;
        _hoverStartTime = -999f;

        if (_spawnRoutine != null) { StopCoroutine(_spawnRoutine); _spawnRoutine = null; }

        if (phoneCanvas != null)
            foreach (Transform child in phoneCanvas)
                if (child.GetComponent<AdPopup>() != null)
                    Destroy(child.gameObject);
    }

    private void HandleGameOver()
    {
        _gameReady = false;
        if (_spawnRoutine != null) { StopCoroutine(_spawnRoutine); _spawnRoutine = null; }
    }

    private void HandleAdOpened() => _adOpen = true;
    private void HandleAdClosed() => _adOpen = false;

    private void HandleItemHovered(HiddenItem _)
    {
        if (!_isHovering)
        {
            _isHovering = true;
            _hoverStartTime = Time.time;
        }
    }

    private void HandleHoverExit()
    {
        _isHovering = false;
        _hoverStartTime = -999f;
    }

    // ── Spawn loop ─────────────────────────────────────────
    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (_gameReady)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            if (!_gameReady || _adOpen) continue;
            if (IsInHoverGrace()) continue;

            SpawnAd();
        }
    }

    private bool IsInHoverGrace()
    {
        if (!_isHovering) return false;
        return (Time.time - _hoverStartTime) < hoverGracePeriod;
    }

    private void SpawnAd()
    {
        if (adPopupPrefab == null || phoneCanvas == null || ads.Count == 0) return;

        // Pick a random entry — image and sound travel together
        AdEntry chosen = ads[Random.Range(0, ads.Count)];

        if (chosen.image == null)
        {
            Debug.LogWarning("[AdPopupManager] Selected ad entry has no image assigned.");
            return;
        }

        var go = Instantiate(adPopupPrefab, phoneCanvas);
        var rect = go.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.zero;
        }

        var popup = go.GetComponent<AdPopup>();
        if (popup != null)
            popup.Show(chosen.image, chosen.sound);  // pass both image and sound
    }
}
