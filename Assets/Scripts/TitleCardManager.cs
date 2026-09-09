using UnityEngine;
using UnityEngine.Playables;

public class TitleCardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject titleCard;
    [SerializeField] private PlayableDirector playableDirector;

    [Header("Player References")]
    [SerializeField] private GameObject cutscenePlayer;
    [SerializeField] private GameObject realPlayer;

    [Header("Audio")]
    [SerializeField] private float introAudioDelay = 1.2f;

    private bool hasStarted = false;

    private void Start()
    {
        // Play title ambience as soon as the title card appears
        AudioManager.Instance.PlayTitleAmbience();
    }

    private void Update()
    {
        if (hasStarted)
            return;

        if (Input.GetButtonDown("Interact"))
        {
            hasStarted = true;

            // Wait for the title fade before starting the intro ambience
            AudioManager.Instance.PlayIntroAmbienceDelayed(introAudioDelay);

            // Make sure the correct player is active
            if (realPlayer != null)
                realPlayer.SetActive(false);

            if (cutscenePlayer != null)
                cutscenePlayer.SetActive(true);

            titleCard.SetActive(false);

            playableDirector.Play();
        }
    }
}