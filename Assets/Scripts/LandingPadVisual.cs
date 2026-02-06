using TMPro;
using UnityEngine;

public class LandingPadVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreTextMesh;

    private void Awake()
    {
        LandingPad landingPad = GetComponent<LandingPad>();
        scoreTextMesh.text = "x" + landingPad.ScoreMultiplier.ToString(); 
    }
}
