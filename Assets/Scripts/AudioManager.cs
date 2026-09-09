using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource environmentSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource cutsceneSource;

    [SerializeField] private AudioClip titleAmbience;
    [SerializeField] private AudioClip introAmbience;
    [SerializeField] private AudioClip mazeAmbience;
    [SerializeField] private AudioClip churchBells;

    [SerializeField] private AudioClip interactClip;
    [SerializeField] private AudioClip statueInteractionBackground;

    [SerializeField] private AudioClip stoneGrinding;
    [SerializeField] private AudioClip jumpscareMusic;
    [SerializeField] private AudioClip screamAudio;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlayAmbience(AudioClip clip, bool loop = true)
    {
        if (clip == null || ambienceSource == null)
            return;

        ambienceSource.clip = clip;
        ambienceSource.loop = loop;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        if (ambienceSource != null)
        {
            ambienceSource.Stop();
            ambienceSource.clip = null;
        }
    }

    public void PlayTitleAmbience()
    {
        PlayAmbience(titleAmbience);
    }

    public void PlayIntroAmbience()
    {
        PlayAmbience(introAmbience);
    }

    public void PlayIntroAmbienceDelayed(float delay)
    {
        StartCoroutine(PlayIntroAfterDelay(delay));
    }

    private IEnumerator PlayIntroAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayIntroAmbience();
    }

    public void PlayChurchBells()
    {
        PlayAmbience(churchBells, true);
    }

    public void StopChurchBells()
    {
        StopAmbience();
    }

    public void PlayMazeAmbience()
    {
        if (environmentSource == null || mazeAmbience == null)
            return;

        if (environmentSource.isPlaying)
            return;

        environmentSource.clip = mazeAmbience;
        environmentSource.loop = true;
        environmentSource.Play();
    }

    public void StopMazeAmbience()
    {
        if (environmentSource != null)
        {
            environmentSource.Stop();
            environmentSource.clip = null;
        }
    }

    public void StopAllBackgroundAudio()
    {
        if (ambienceSource != null)
        {
            ambienceSource.Stop();
            ambienceSource.clip = null;
        }

        if (environmentSource != null)
        {
            environmentSource.Stop();
            environmentSource.clip = null;
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayInteract()
    {
        if (interactClip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(interactClip);
    }

    public void PlayStatueInteractionBackground()
    {
        if (statueInteractionBackground == null ||
            cutsceneSource == null)
            return;

        cutsceneSource.Stop();
        cutsceneSource.clip = statueInteractionBackground;
        cutsceneSource.loop = true;
        cutsceneSource.Play();
    }

    public void StopStatueInteractionBackground()
    {
        if (cutsceneSource == null)
            return;

        if (cutsceneSource.clip == statueInteractionBackground)
        {
            cutsceneSource.Stop();
            cutsceneSource.clip = null;
        }
    }

    public void PlayStoneGrinding()
    {
        if (stoneGrinding == null || cutsceneSource == null)
            return;

        cutsceneSource.Stop();
        cutsceneSource.clip = stoneGrinding;
        cutsceneSource.loop = false;
        cutsceneSource.Play();
    }

    public void StopStoneGrinding()
    {
        if (cutsceneSource == null)
            return;

        if (cutsceneSource.clip == stoneGrinding)
        {
            cutsceneSource.Stop();
            cutsceneSource.clip = null;
        }
    }

    public void PlayJumpscareMusic()
    {
        if (jumpscareMusic == null || cutsceneSource == null)
            return;

        cutsceneSource.Stop();
        cutsceneSource.clip = jumpscareMusic;
        cutsceneSource.time = 30f;
        cutsceneSource.loop = false;
        cutsceneSource.Play();
    }

    public void PlayScream()
    {
        if (screamAudio == null || cutsceneSource == null)
            return;

        cutsceneSource.PlayOneShot(screamAudio);
    }
}