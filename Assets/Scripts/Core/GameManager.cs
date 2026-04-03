using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[DefaultExecutionOrder(-200)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, Paused, MissionComplete, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Menu;

    public event Action<GameState> OnGameStateChanged;

    [Header("Mission Settings")]
    public int currentMissionIndex = 0;
    public MissionData[] missions;

    private int score;
    private int totalTargets;
    private int destroyedTargets;

    public int Score => score;
    public int TotalTargets => totalTargets;
    public int DestroyedTargets => destroyedTargets;
    public float DestructionPercentage => totalTargets > 0 ? (float)destroyedTargets / totalTargets * 100f : 0f;

    void Awake()
    {
        Instance = this;
    }

    public void StartMission()
    {
        score = 0;
        destroyedTargets = 0;
        totalTargets = 0;
        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    public void RegisterTarget()
    {
        totalTargets++;
    }

    public void TargetDestroyed(int points)
    {
        destroyedTargets++;
        score += points;

        if (destroyedTargets >= totalTargets && totalTargets > 0)
        {
            Invoke(nameof(DoMissionComplete), 1.5f);
        }
    }

    void DoMissionComplete()
    {
        if (CurrentState != GameState.Playing) return;
        SetState(GameState.MissionComplete);
        int coins = score;
        int gems = DestructionPercentage >= 100f ? 4 : 2;
        CurrencyManager.Instance?.AddCoins(coins);
        CurrencyManager.Instance?.AddGems(gems);
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void PlayerDied()
    {
        SetState(GameState.GameOver);
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            Time.timeScale = 0f;
            SetState(GameState.Paused);
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            Time.timeScale = 1f;
            SetState(GameState.Playing);
        }
    }

    public void RestartMission()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMission(int index)
    {
        currentMissionIndex = index;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}
