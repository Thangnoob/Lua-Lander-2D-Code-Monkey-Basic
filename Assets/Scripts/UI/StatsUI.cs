using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsTextMesh;
    [SerializeField] private Image speedUpArrowImage;
    [SerializeField] private Image speedDownArrowImage;
    [SerializeField] private Image speedLeftArrowImage;
    [SerializeField] private Image speedRightArrowImage;
    [SerializeField] private Image fuelImage;

    private void Update()
    {
        UpdateStatsTextMesh();
    }   

    private void UpdateStatsTextMesh()
    {
        fuelImage.fillAmount = Lander.Instance.GetFuelNormalized();

        speedDownArrowImage.enabled = Lander.Instance.GetSpeedY() < -1f;
        speedUpArrowImage.enabled = Lander.Instance.GetSpeedY() > 1f;
        speedLeftArrowImage.enabled = Lander.Instance.GetSpeedX() < -1f;
        speedRightArrowImage.enabled = Lander.Instance.GetSpeedX() > 1f;

        statsTextMesh.text = GameManager.Instance.GetScore().ToString() + "\n" 
            + Mathf.Round(GameManager.Instance.GetTime()).ToString() + "\n"
            + Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedX())).ToString() + "\n"
            + Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedY())).ToString() + "\n";
    }
}
