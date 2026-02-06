using UnityEngine;

public class LanderVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem leftThrusterParticleSystem;
    [SerializeField] private ParticleSystem middleThrusterParticleSystem;
    [SerializeField] private ParticleSystem rightThrusterParticleSystem;
    [SerializeField] private GameObject landerExploreVfx;

    private Lander lander;

    private void Awake()
    {
        lander = GetComponent<Lander>();
        
        Lander.Instance.OnUpForce += Lander_OnUpForce;
        Lander.Instance.OnLeftForce += Lander_OnLeftForce;
        Lander.Instance.OnRightForce += Lander_OnRightForce;
        Lander.Instance.OnBeforeForce += Lander_OnBeforeForce;

        SetEnableThrusterParticleSystem(leftThrusterParticleSystem, false);
        SetEnableThrusterParticleSystem(middleThrusterParticleSystem, false);
        SetEnableThrusterParticleSystem(rightThrusterParticleSystem, false);
    }

    private void Start()
    {
        lander.OnLanded += Lander_OnLanded;
    }

    private void OnDestroy()
    {
        Lander.Instance.OnUpForce -= Lander_OnUpForce;
        Lander.Instance.OnLeftForce -= Lander_OnLeftForce;
        Lander.Instance.OnRightForce -= Lander_OnRightForce;
        Lander.Instance.OnBeforeForce -= Lander_OnBeforeForce;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch(e.landingType)
        {
            case Lander.LandingType.WrongLandingArea:
            case Lander.LandingType.TooSteepAngle:
            case Lander.LandingType.TooFastLanding:
                Instantiate(landerExploreVfx, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                break;
            case Lander.LandingType.Success:
                break;

        }
    }

    private void Lander_OnUpForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(middleThrusterParticleSystem, true);
        SetEnableThrusterParticleSystem(leftThrusterParticleSystem, true);
        SetEnableThrusterParticleSystem(rightThrusterParticleSystem, true);
    }

    private void Lander_OnLeftForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(leftThrusterParticleSystem, true);
    }

    private void Lander_OnRightForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(rightThrusterParticleSystem, true);
    }

    private void Lander_OnBeforeForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(leftThrusterParticleSystem, false);
        SetEnableThrusterParticleSystem(middleThrusterParticleSystem, false);
        SetEnableThrusterParticleSystem(rightThrusterParticleSystem, false);
    }

    private void SetEnableThrusterParticleSystem(ParticleSystem thrusterParticleSystem, bool enable)
    {
        ParticleSystem.EmissionModule emissionModule = thrusterParticleSystem.emission;
        emissionModule.enabled = enable;
    }
}
