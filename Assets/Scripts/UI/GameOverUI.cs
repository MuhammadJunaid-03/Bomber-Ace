using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject missionCompletePanel;
    public GameObject pausePanel;

    [Header("Game Over")]
    public TextMeshProUGUI gameOverScoreText;
    public Button retryButton;
    public Button goMenuButton;

    [Header("Mission Complete")]
    public TextMeshProUGUI missionScoreText;
    public TextMeshProUGUI destructionText;
    public TextMeshProUGUI coinsEarnedText;
    public TextMeshProUGUI gemsEarnedText;
    public Image[] stars;
    public Button nextMissionButton;
    public Button backToMenuButton;

    [Header("Pause")]
    public Button resumeButton;
    public Button pauseRetryButton;
    public Button pauseMenuButton;

    void Start()
    {
        HideAll();

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

        // Buttons
        if (retryButton != null) retryButton.onClick.AddListener(() => GameManager.Instance?.RestartMission());
        if (goMenuButton != null) goMenuButton.onClick.AddListener(() => GameManager.Instance?.GoToMainMenu());
        if (nextMissionButton != null) nextMissionButton.onClick.AddListener(OnNextMission);
        if (backToMenuButton != null) backToMenuButton.onClick.AddListener(() => GameManager.Instance?.GoToMainMenu());
        if (resumeButton != null) resumeButton.onClick.AddListener(() => GameManager.Instance?.ResumeGame());
        if (pauseRetryButton != null) pauseRetryButton.onClick.AddListener(() => GameManager.Instance?.RestartMission());
        if (pauseMenuButton != null) pauseMenuButton.onClick.AddListener(() => GameManager.Instance?.GoToMainMenu());
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
                    GameManager.Instance.PauseGame();
                else if (GameManager.Instance.CurrentState == GameManager.GameState.Paused)
                    GameManager.Instance.ResumeGame();
            }
        }
    }

    void OnGameStateChanged(GameManager.GameState state)
    {
        HideAll();

        switch (state)
        {
            case GameManager.GameState.GameOver:
                ShowGameOver();
                break;
            case GameManager.GameState.MissionComplete:
                ShowMissionComplete();
                break;
            case GameManager.GameState.Paused:
                ShowPause();
                break;
        }
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverScoreText != null)
                gameOverScoreText.text = $"Score: {GameManager.Instance?.Score ?? 0}";
        }
    }

    void ShowMissionComplete()
    {
        if (missionCompletePanel == null) return;
        missionCompletePanel.SetActive(true);

        var gm = GameManager.Instance;
        if (gm == null) return;

        if (missionScoreText != null) missionScoreText.text = $"Score: {gm.Score}";
        if (destructionText != null) destructionText.text = $"Destruction: {gm.DestructionPercentage:F0}%";

        int coins = gm.Score;
        int gems = gm.DestructionPercentage >= 100f ? 4 : 2;
        if (coinsEarnedText != null) coinsEarnedText.text = $"+{coins}";
        if (gemsEarnedText != null) gemsEarnedText.text = $"+{gems}";

        // Stars based on destruction
        if (stars != null)
        {
            int starCount = 0;
            if (gm.DestructionPercentage >= 30f) starCount = 1;
            if (gm.DestructionPercentage >= 70f) starCount = 2;
            if (gm.DestructionPercentage >= 100f) starCount = 3;

            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                    stars[i].color = i < starCount ? Color.yellow : Color.gray;
            }
        }

        // Unlock next mission
        int next = gm.currentMissionIndex + 1;
        int highest = PlayerPrefs.GetInt("HighestMission", 0);
        if (next > highest)
        {
            PlayerPrefs.SetInt("HighestMission", next);
            PlayerPrefs.Save();
        }
    }

    void ShowPause()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    void HideAll()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (missionCompletePanel != null) missionCompletePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void OnNextMission()
    {
        if (GameManager.Instance != null)
        {
            int next = GameManager.Instance.currentMissionIndex + 1;
            if (next < GameManager.Instance.missions.Length)
                GameManager.Instance.LoadMission(next);
            else
                GameManager.Instance.GoToMainMenu();
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
}
