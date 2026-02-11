using System;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    public const float GRAVITY_NORMAL = 0.7f;
    public static Lander Instance { get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler OnFuelPickup;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public LandingType landingType;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier;
        public int score;
    }

    public enum LandingType
    {
        Success,
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding,
    }

    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver,
    }

    private Rigidbody2D landerRigidbody2D;
    private float fuelAmount;
    private float maxFuelAmount = 10f;
    private State state;

    private void Awake()
    {
        Instance = this;

        fuelAmount = maxFuelAmount;
        state = State.WaitingToStart;

        landerRigidbody2D = GetComponent<Rigidbody2D>();
        landerRigidbody2D.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (GameInput.Instance.IsUpActionPressed() ||
                    GameInput.Instance.IsRightActionPressed() ||
                    GameInput.Instance.IsLeftActionPressed() ||
                    GameInput.Instance.GetMovementInputVector2() != Vector2.zero)
                {
                    landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
                    SetState(State.Normal);
                }
                break;
            case State.Normal:
                if (fuelAmount <= 0f)
                {
                    return;
                }

                if (GameInput.Instance.IsUpActionPressed() ||
                    GameInput.Instance.IsRightActionPressed() ||
                    GameInput.Instance.IsLeftActionPressed())
                {
                    ConsumeFuel();
                    landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
                }

                float gamepadDeadzone = 0.4f;
                if (GameInput.Instance.IsUpActionPressed() || GameInput.Instance.GetMovementInputVector2().y > 0f)
                {
                    float force = 700f;
                    landerRigidbody2D.AddForce(transform.up * force * Time.fixedDeltaTime);
                    OnUpForce?.Invoke(this, EventArgs.Empty);
                }
                if (GameInput.Instance.IsRightActionPressed() || GameInput.Instance.GetMovementInputVector2().x < -gamepadDeadzone)
                {
                    float turnSpeed = -100f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);
                    OnRightForce?.Invoke(this, EventArgs.Empty);
                }
                if (GameInput.Instance.IsLeftActionPressed() || GameInput.Instance.GetMovementInputVector2().x > gamepadDeadzone)
                {
                    float turnSpeed = 100f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);
                    OnLeftForce?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision2d)
    {
        if (!collision2d.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.WrongLandingArea,
                dotVector = 0f,
                landingSpeed = 0f,
                scoreMultiplier = 0,
                score = 0,
            });
            SetState(State.GameOver);
            return;
        }

        float softLandingVelocityMagnitude = 4f;
        float relativeVelocityMagnitude = collision2d.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooFastLanding,
                dotVector = 0f,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = landingPad.ScoreMultiplier,
                score = 0,
            });
            SetState(State.GameOver);
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVectorForSafeLanding = .90f;
        if (dotVector < minDotVectorForSafeLanding)
        {
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooSteepAngle,
                dotVector = dotVector,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = landingPad.ScoreMultiplier,
                score = 0,
            });
            return;
        }

        float maxScoreAmountLandingAngel = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngelScore = maxScoreAmountLandingAngel - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngel;

        float maxScoreAmountLandingSpeed = 100;
        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

        int score = Mathf.RoundToInt((landingAngelScore + landingSpeedScore) * landingPad.ScoreMultiplier);

        OnLanded?.Invoke(this, new OnLandedEventArgs{
            landingType = LandingType.Success,
            dotVector = dotVector,
            landingSpeed = relativeVelocityMagnitude,   
            scoreMultiplier = landingPad.ScoreMultiplier,
            score = score, 
        });
        SetState(State.GameOver);
    }

    private void OnTriggerEnter2D(Collider2D collision2d)
    {
        if (collision2d.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
            AddFuelAndNomalize();
            OnFuelPickup?.Invoke(this, EventArgs.Empty);
            fuelPickup.DestroySelf();
        }

        if (collision2d.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }


    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
    }

    private void AddFuelAndNomalize()
    {
        float addedFuelAmount = 10f;
        fuelAmount += addedFuelAmount;  
        if (fuelAmount > maxFuelAmount)
        {
            fuelAmount = maxFuelAmount;
        }
    }

    private void ConsumeFuel()
    {
        float fuelConsumptionAmount = 1f;
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }   

    public float GetFuelAmount()
    {
        return fuelAmount;
    }

    public float GetSpeedX()
    {
        return landerRigidbody2D.linearVelocity.x;
    }

    public float GetSpeedY()
    {
        return landerRigidbody2D.linearVelocity.y;
    }

    public float GetFuelNormalized()
    {
        return fuelAmount / maxFuelAmount;
    }
}
