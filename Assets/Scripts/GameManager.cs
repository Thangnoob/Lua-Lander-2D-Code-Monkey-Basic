using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
}
