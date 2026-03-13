using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Progress")]
    public TextMeshProUGUI progressText;

    [Header("Win Screen")]
    public GameObject winScreen;
    public TextMeshProUGUI winText;

    private void OnEnable()
    {
        GameEvents.OnGameStart += OnGameStart;
        GameEvents.OnProgressUpdated += OnProgressUpdated;
        GameEvents.OnAllItemsFound += OnAllFound;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStart -= OnGameStart;
        GameEvents.OnProgressUpdated -= OnProgressUpdated;
        GameEvents.OnAllItemsFound -= OnAllFound;
    }

    private void OnGameStart()
    {
        winScreen.SetActive(false);
        RefreshProgress();
    }

    private void OnProgressUpdated(int found, int total)
    {
        if (progressText) progressText.text = $"{found} / {total}";
    }

    private void OnAllFound()
    {
        winScreen.SetActive(true);
        if (winText) winText.text = "All items found! Ready to Leave?";

        Time.timeScale = 0f;

        Cursor.visible = true;

        var cursor = SmartphoneCursor.Instance;
        if (cursor != null && cursor.revealMaterial != null)
            cursor.revealMaterial.SetFloat("_RevealRadius", 0f);
    }

    private void RefreshProgress()
    {
        var gm = GameManager.Instance;
        if (progressText) progressText.text = $"{gm.FoundCount} / {gm.TotalItems}";
    }
}
