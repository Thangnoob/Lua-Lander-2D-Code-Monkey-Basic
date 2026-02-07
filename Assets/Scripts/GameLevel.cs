using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    [SerializeField] private Transform landerStartPosition;

    public int GetLevelNumber() => levelNumber;

    public Vector3 GetLanderStartPosion()
    {
        return landerStartPosition.position;
    }
}
