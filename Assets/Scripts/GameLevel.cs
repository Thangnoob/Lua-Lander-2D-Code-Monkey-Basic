using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    [SerializeField] private Transform landerStartPosition;
    [SerializeField] private Transform cameraStartTargetTranform;
    [SerializeField] private float zoomOutOrthographicSize;

    public int GetLevelNumber() => levelNumber;

    public Vector3 GetLanderStartPosion()
    {
        return landerStartPosition.position;
    }

    public Transform GetCameraStartTargetTransform()
    {
        return cameraStartTargetTranform;
    }

    public float GetZoomOutOrthographicSize()
    {
        return zoomOutOrthographicSize;
    }
}
