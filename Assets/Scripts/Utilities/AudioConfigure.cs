using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Configure", menuName = "Game/Audio")]
public class AudioConfigure : ScriptableObject
{
    public AudioConfiguration[] audioConfigurations;

    [System.Serializable]
    public class AudioConfiguration
    {
        public string audioName;
        public AudioClip audioClip;

        [Range(0, 1)]
        public float volume;
        [Range(.1f, 3)]
        public float pitch;
        public float minPitch;
        public float maxPitch;


    }

    public AudioClip GetAudioClip(string name)
    {
        var config = GetAudioConfiguration(name);
        return config?.audioClip;
    }

    public AudioConfiguration GetAudioConfiguration(string name)
    {
        foreach (var audio in audioConfigurations)
        {
            if (audio.audioName == name)
            {
                return audio;
            }
        }
        return null;
    }
}
