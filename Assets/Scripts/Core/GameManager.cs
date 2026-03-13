using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<HiddenItem> allItems = new();

    public bool IsGameActive { get; private set; }
    public int FoundCount => _foundItems.Count;
    public int TotalItems => allItems.Count;
    public float ProgressNorm => TotalItems > 0 ? (float)FoundCount / TotalItems : 0f;

    private HashSet<HiddenItem> _foundItems = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable() => GameEvents.OnItemFound += HandleItemFound;
    private void OnDisable() => GameEvents.OnItemFound -= HandleItemFound;

    private void Start()
    {
        if (allItems.Count == 0)
            allItems = FindObjectsByType<HiddenItem>(FindObjectsSortMode.None).ToList();
        StartGame();
    }

    public void StartGame()
    {
        _foundItems.Clear();
        IsGameActive = true;
        GameEvents.GameStart();
    }

    public void ResetGame()
    {
        foreach (var item in allItems) item.ResetItem();
        IsGameActive = false;
        GameEvents.GameReset();
        StartGame();
    }

    private void HandleItemFound(HiddenItem item)
    {
        if (!_foundItems.Add(item)) return;

        Debug.Log($"Found: {item.itemData?.itemName} ({FoundCount}/{TotalItems})");
        GameEvents.ProgressUpdated(FoundCount, TotalItems);

        if (FoundCount >= TotalItems)
        {
            IsGameActive = false;
            GameEvents.AllItemsFound();
        }
    }
}