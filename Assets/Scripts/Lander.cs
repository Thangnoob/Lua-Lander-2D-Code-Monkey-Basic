using System;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    public static Lander Instance { get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
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


    private Rigidbody2D landerRigidbody2D;
    private float fuelAmount;
    private float maxFuelAmount = 10f;

    private void Awake()
    {
        fuelAmount = maxFuelAmount;
        Instance = this;
        landerRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        if (fuelAmount <= 0f)
        {
            return;
        }

        if (Keyboard.current.upArrowKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        { 
            ConsumeFuel();
        
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            float force = 700f;
            landerRigidbody2D.AddForce(transform.up * force * Time.fixedDeltaTime);
            OnUpForce?.Invoke(this, EventArgs.Empty);
        } 
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            float turnSpeed = -100f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);
            OnRightForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            float turnSpeed = 100f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);
            OnLeftForce?.Invoke(this, EventArgs.Empty);
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

        //CalculateTheScore();
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
    }

    private void OnTriggerEnter2D(Collider2D collision2d)
    {
        if (collision2d.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
            AddFuelAndNomalize();
            fuelPickup.DestroySelf();
        }

        if (collision2d.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
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
