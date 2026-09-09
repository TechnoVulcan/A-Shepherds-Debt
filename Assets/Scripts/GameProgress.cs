using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    [Header("Progress Flags")]
    public bool talkedToGirl = false;
    public bool talkedToStatue = false;
    public bool girlMissing = false;
    public bool NoExit = false;

    [Header("Karma")]
    public int karma = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetKarma(int value)
    {
        karma = value;

        Debug.Log("Karma set to: " + karma);
    }
}