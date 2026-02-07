using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static int currentLevel = 1;
    [SerializeField] private List<GameLevel> gameLevels;


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

        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        foreach (GameLevel level in gameLevels)
        {
            if (level.GetLevelNumber() == currentLevel)
            {
                Debug.Log("Loading level " + currentLevel);
                Debug.Log("Level found: " + level.GetLevelNumber());
                GameLevel spawnedLevel = Instantiate(level, Vector3.zero, Quaternion.identity);
                Lander.Instance.transform.position = spawnedLevel.GetLanderStartPosion();
            }
        }
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

    public void GoToNextLevel()
    {
        currentLevel++;
        SceneManager.LoadScene(0);
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(0);
    }
}
