using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private string gameplaySceneName = "Game"; // change to your scene
    [SerializeField] private GameObject howToPanel;
    [SerializeField] private GameObject firstMenuButton;
    [SerializeField] private GameObject firstHowToButton;

    void Start()
    {
        if (howToPanel != null) howToPanel.SetActive(false);
        if (firstMenuButton != null)
            UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(firstMenuButton);
    }

    public void Play()
    {
        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            Debug.LogError("MainMenu: gameplaySceneName is empty.");
            return;
        }
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        Debug.Log("Exit is not supported on WebGL.");
#else
        Application.Quit();
#endif
    }

    public void ShowHowTo()
    {
        if (!howToPanel) return;
        howToPanel.SetActive(true);
        if (firstHowToButton != null)
            UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(firstHowToButton);
    }

    public void HideHowTo()
    {
        if (!howToPanel) return;
        howToPanel.SetActive(false);
        if (firstMenuButton != null)
            UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(firstMenuButton);
    }
}
