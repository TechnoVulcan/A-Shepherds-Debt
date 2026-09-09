using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentState = 0;

    private void Awake()
    {
        Instance = this;
    }
}