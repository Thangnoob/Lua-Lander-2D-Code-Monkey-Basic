using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int score;
    [SerializeField] private int coinValue = 500;

    private float time;

    private void Update()
    {
        time += Time.deltaTime;
    }

    private void Start()
    {
        Instance = this;

        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
    }

    private void OnDestroy()
    {
        Lander.Instance.OnCoinPickup -= Lander_OnCoinPickup;
    }   

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AddScore(e.score);
    }

    private void Lander_OnCoinPickup(object sender, System.EventArgs e)
    {
        AddScore(coinValue);
        Debug.Log("Pickup: " + score);
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
