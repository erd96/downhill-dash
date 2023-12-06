using System.Collections;
using UnityEngine;

[System.Serializable]
public class AudioSettings
{
    public float volume;
    public float pitch;
    public bool loop;

    public AudioSettings(float volume, float pitch, bool loop)
    {
        this.volume = volume;
        this.pitch = pitch;
        this.loop = loop;
    }
}

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip idleClip;
    [SerializeField] AudioClip switchLaneClip;
    [SerializeField] AudioClip jumpLandSnow;
    [SerializeField] AudioClip railClip;

    private bool didJump = false;

    private void PlayAudioSequentially(AudioClip firstClip, AudioClip secondClip)
    {
        StartCoroutine(PlayAudioSequentiallyCoroutine(firstClip, secondClip));
    }

    private IEnumerator PlayAudioSequentiallyCoroutine(AudioClip firstClip, AudioClip secondClip)
    {
        // Play the first clip
        PlayAudioClip(firstClip);

        // Wait for the first clip to finish
        yield return new WaitForSeconds(audioSource.clip.length);

        // Play the second clip
        PlayAudioClip(secondClip);
    }

    private void PlayAudioClip(AudioClip clip)
    {
        AudioSettings settings = GetSettingsForClip(clip);

        audioSource.loop = settings.loop;
        audioSource.clip = clip;
        audioSource.volume = settings.volume;
        audioSource.pitch = settings.pitch;
        audioSource.Play();
    }

    private AudioSettings GetSettingsForClip(AudioClip clip)
    {

        if (clip == jumpLandSnow)
        {
            return new AudioSettings(0.5f, 1f, false);
        }
        else if (clip == switchLaneClip)
        {
            return new AudioSettings(0.2f, 0.9f, false);
        }
        else if (clip == railClip)
        {
            return new AudioSettings(0.2f, 1f, false);
        }
        else
        {
            return new AudioSettings(0.1f, 0.7f, true);
        }
    }

    public void PlayIdleAudioClip()
    {
        if (didJump)
        {
            PlayAudioSequentially(jumpLandSnow, idleClip);
            didJump = false;
        }
        else if (audioSource.clip != idleClip)
        {
            PlayAudioClip(idleClip);
        }
    }

    public void PlaySwitchLaneAudioClip()
    {
        if (didJump)
        {
            PlayAudioSequentially(jumpLandSnow, switchLaneClip);
            didJump = false;
        }
        else if (audioSource.clip != switchLaneClip)
        {
            PlayAudioClip(switchLaneClip);
        }
    }

    public void PlayRailAudioClip()
    {
        didJump = false;
        if (audioSource.clip != railClip) PlayAudioClip(railClip);

    }
    public void PlayJumpAudioClip()
    {
        didJump = true;
    }

    public void JumpLandingAudio()
    {
        if (audioSource.clip != switchLaneClip) PlayAudioClip(jumpLandSnow);
    }


}
