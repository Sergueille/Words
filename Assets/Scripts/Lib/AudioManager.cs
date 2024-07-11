using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound {
    public AudioClip[] clips;
    public float volume = 1.0f; 
    public float pitch = 1.0f;
    public float pitchVariation = 0.0f;
    public bool looping = false;

    public AudioManager.SoundHandle Play() {
        return AudioManager.i.PlaySoundInstance(this);
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager i;
    
    public float masterVolume = 1;
    public int poolSize = 20;

    private AudioSource[] audioSources;

    public AudioClip[] clips;

    private void Awake()
    {
        i = this;

        audioSources = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            audioSources[i] = new GameObject("Audio Source").AddComponent<AudioSource>();
        }
    }

    public static SoundHandle PlaySound(Sound s)
    {
        return i.PlaySoundInstance(s);
    }

    public static SoundHandle PlaySound(string clipName, float volume = 1, float pitch = 1, float pitchVariation = 0, bool loop = false)
    {
        foreach (AudioClip clip in i.clips) // OPTI: use a map or something
        {
            if (clip.name == clipName)
            {
                return i.PlaySoundInstance(new Sound {
                    clips = new AudioClip[] { clip },
                    volume = volume,
                    pitch = pitch,
                    pitchVariation = pitchVariation,
                    looping = loop
                });
            }
        }

        Debug.LogError("No clip found with name " + clipName);
        return null;
    }

    public SoundHandle PlaySoundInstance(Sound s)
    {
        int pos = 0;
        for (pos = 0; pos < poolSize; pos++) // Loop through all sources
        {
            if (!audioSources[pos].isPlaying) // Find one that isn't playing
                break;
        }

        if (pos == poolSize) // All already playing
        {
            Debug.LogError("Audio sources pool size exceeded");

            pos = 0;
            float bestImportance = 100000;
            for (int i = 0; i < poolSize; i++)
            {
                float importance = (audioSources[i].loop ? 100 : 0) + audioSources[i].clip.length;

                if (importance < bestImportance)
                {
                    bestImportance = importance; // Find the least important sound to replace
                    pos = i;
                }
            }
        }

        AudioSource source = audioSources[pos];

        AudioClip clip = s.clips[Random.Range(0, s.clips.Length)]; // Pick a random clip

        source.spatialBlend = 0;
        source.clip = clip;
        source.volume = masterVolume * s.volume;
        source.pitch = s.pitch + Random.Range(-s.pitchVariation, s.pitchVariation);
        source.loop = s.looping;
        source.Play();

        return new SoundHandle {
            source = source,
            audioClip = clip,
        };
    }

    public class SoundHandle
    {
        public AudioClip audioClip;
        public AudioSource source;

        public void Stop()
        {
            if (source != null && source.clip == audioClip)
            {
                source.Stop();
            }
        }

        public void FadeAndStop(float duration)
        {
            if (source == null || source.clip != audioClip || !source.isPlaying) 
                return;

            AudioSource capturedSource = source;

            LeanTween.value(source.volume, 0, duration).setOnUpdate(t => {
                capturedSource.volume = t;
            });
        }
    }

}