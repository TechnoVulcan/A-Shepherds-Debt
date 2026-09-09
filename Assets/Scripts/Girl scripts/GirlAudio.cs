using UnityEngine;

public class GirlAudio : MonoBehaviour
{
    [SerializeField] private AudioSource cryingSource;

    public void StartCrying()
    {
        cryingSource.Play();
    }

    public void StopCrying()
    {
        cryingSource.Stop();
    }
}