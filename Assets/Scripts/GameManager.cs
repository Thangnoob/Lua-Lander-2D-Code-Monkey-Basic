using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event EventHandler OnGamePaused; 
    public event EventHandler OnGameUnPaused;

    public static int currentLevel = 1;
    public static int totalScore = 0;

    public static void ResetGame()
    {
        currentLevel = 1;
        totalScore = 0;
    }

    [SerializeField] private List<GameLevel> gameLevels;
    [SerializeField] private CinemachineCamera cinemachineCamera;     


    private int score;
    [SerializeField] private int coinValue = 500;

    private float time;
    private bool isTimerActive;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {         
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnStateChanged += Lander_OnStateChanged;

        GameInput.Instance.OnMenuButtonPressed += GameInput_OnMenuButtonPressed;
        LoadCurrentLevel();
    }

    private void GameInput_OnMenuButtonPressed(object sender, EventArgs e)
    {
        PauseUnpauseGame();
    }

    private void LoadCurrentLevel()
    {
        GameLevel level = GetGameLevel();
        GameLevel spawnedLevel = Instantiate(level, Vector3.zero, Quaternion.identity);
        Lander.Instance.transform.position = spawnedLevel.GetLanderStartPosion();
        cinemachineCamera.Target.TrackingTarget = spawnedLevel.GetCameraStartTargetTransform();
        CinemachineCameraZoom2D.Instance.SetTargetOrthographicSize(spawnedLevel.GetZoomOutOrthographicSize());
        
    }

    private GameLevel GetGameLevel()
    {
        foreach (GameLevel level in gameLevels)
        {
            if (level.GetLevelNumber() == currentLevel)
            {
                return level;
            }
        }
        return null;
    }

    private void OnDestroy()
    {
        Lander.Instance.OnCoinPickup -= Lander_OnCoinPickup;
    }

    private void Update()
    {
        if (isTimerActive)
        {
            time += Time.deltaTime;
        }
    }
    private void Lander_OnStateChanged(object sender, Lander.OnStateChangedEventArgs e)
    {
        isTimerActive = e.state == Lander.State.Normal;

        if (e.state == Lander.State.Normal)
        {
            cinemachineCamera.Target.TrackingTarget = Lander.Instance.transform;
            CinemachineCameraZoom2D.Instance.SetNormalOrthographicSize();
        }
    }
    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AddScore(e.score);
    }

    private void Lander_OnCoinPickup(object sender, System.EventArgs e)
    {
        AddScore(coinValue);
    }

    private void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
    }

    public int GetScore()
    {
        return score;
    }

    public float GetTime()
    {
        return time;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    public void GoToNextLevel()
    {
        currentLevel++;
        totalScore += score;

        if (GetGameLevel() == null)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameOverScene);
        } else
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);

        }
    }

    private void CheckNextLevel()
    {

    }

    public void RetryLevel()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene); 
    }

    private void PauseUnpauseGame()
    {
        if (Time.timeScale == 1f)
        {
            PauseGame();
        }
        else
        {
            UnPauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        OnGameUnPaused?.Invoke(this, EventArgs.Empty);
    }
}
