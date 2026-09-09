using System.Collections;
using UnityEngine;

public class MazeRandomAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource randomSource;

    [Header("Random Sounds")]
    [SerializeField] private AudioClip[] randomSounds;

    [Header("Crow")]
    [SerializeField] private AudioClip crowSound;
    [Range(0f, 1f)]
    [SerializeField] private float crowChance = 0.35f;

    [Header("Timing")]
    [SerializeField] private float minDelay = 6f;
    [SerializeField] private float maxDelay = 14f;

    [Header("Variation")]
    [SerializeField] private float minVolume = 0.45f;
    [SerializeField] private float maxVolume = 0.7f;

    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    private Coroutine soundRoutine;
    private int lastPlayedIndex = -1;

    public void StartRandomSounds()
    {
        if (soundRoutine == null)
            soundRoutine = StartCoroutine(RandomLoop());
    }

    public void StopRandomSounds()
    {
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
            soundRoutine = null;
        }

        randomSource.Stop();
        lastPlayedIndex = -1;
    }

    private IEnumerator RandomLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            if (randomSounds == null || randomSounds.Length == 0)
                continue;

            AudioClip clip;

            // Decide whether to play the crow
            if (crowSound != null && Random.value < crowChance)
            {
                // Crow is allowed to repeat
                clip = crowSound;
            }
            else
            {
                int index;

                // Prevent normal sounds from repeating
                if (randomSounds.Length == 1)
                {
                    index = 0;
                }
                else
                {
                    do
                    {
                        index = Random.Range(0, randomSounds.Length);
                    }
                    while (index == lastPlayedIndex);
                }

                lastPlayedIndex = index;
                clip = randomSounds[index];
            }

            randomSource.volume = Random.Range(minVolume, maxVolume);
            randomSource.pitch = Random.Range(minPitch, maxPitch);

            randomSource.PlayOneShot(clip);
        }
    }
}