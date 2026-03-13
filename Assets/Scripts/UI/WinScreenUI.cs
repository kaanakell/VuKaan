using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    public void OnYesClicked()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnNoClicked()
    {
        Time.timeScale = 1f;

        Cursor.visible = false;

        GameManager.Instance.ResetGame();

        MusicManager.Instance?.FadeIn();
    }
}
