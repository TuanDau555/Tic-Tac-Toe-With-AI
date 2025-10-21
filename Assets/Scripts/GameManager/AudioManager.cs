using UnityEngine;

public class AudioManager : SingletonPersistent<AudioManager>
{
    [Header("Audio List")]
    [SerializeField] private AudioConfigure audioConfigure;

    [Header("Audio Source")]
    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private AudioSource sfxSource;


    public override void Awake()
    {
        base.Awake();
        PlayBackground();
    }

    private void PlayBackground()
    {
        backgroundSource.loop = true;
        backgroundSource.playOnAwake = false;
        backgroundSource.volume = 1f;

        AudioClip backgroundMusic = audioConfigure.GetAudioClip("Main Theme");
        backgroundSource.clip = backgroundMusic;
        backgroundSource.Play();
    }

    public void XClick()
    {
        PlaySfx("O Click Sfx");
    }

    public void OClick()
    {
        PlaySfx("X Click Sfx");
    }

    public void UIClick()
    {
        PlaySfx("UI Click Sfx");
    }

    private void PlaySfx(string audioName)
    {
        if (audioConfigure == null || sfxSource == null) return;

        var audioConfig = audioConfigure.GetAudioConfiguration(audioName);

        if (audioConfig != null && audioConfig.audioClip != null)
        {
            sfxSource.pitch = Random.Range(1f, 3f);
            sfxSource.PlayOneShot(audioConfig.audioClip, audioConfig.volume);
        }
    }
}
