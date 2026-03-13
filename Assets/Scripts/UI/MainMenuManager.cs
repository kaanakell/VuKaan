using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Exact name of your gameplay scene as it appears in Build Settings.")]
    public string gameSceneName = "GameScene";

    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject creditsPanel;

    private void Start()
    {
        Cursor.visible   = true;
        Cursor.lockState = CursorLockMode.None;

        ShowMain();
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnCreditsClicked() => ShowCredits();

    public void OnBackClicked() => ShowMain();

    private void ShowMain()
    {
        if (mainPanel)    mainPanel.SetActive(true);
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    private void ShowCredits()
    {
        if (mainPanel)    mainPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(true);
    }
}
