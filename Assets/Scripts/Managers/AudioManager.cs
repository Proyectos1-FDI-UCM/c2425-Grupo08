using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SFXType
{
    Breath,
    Walk,
    Jump,
    Fall,
    GameOver
    // Añadir más sonidos aquí según sea necesario
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private List<SFXClip> sfxClips;
    [SerializeField] private AudioSource _sfxSource;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private List<AudioSource> loopingSources = new List<AudioSource>();
    private List<SFXType> activeLoopingSFX = new List<SFXType>();

    [System.Serializable]
    public struct SFXClip
    {
        public SFXType type;
        public AudioClip clip;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySFX(SFXType type, bool loop = false)
    {
        AudioClip clip = GetSFXClip(type);
        if (clip == null) return;

        if (loop)
        {
            PlayLoopingSFX(type);
            return;
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = clip;
        newSource.outputAudioMixerGroup = _sfxSource.outputAudioMixerGroup;
        newSource.volume = sfxVolume;
        newSource.Play();
    }

    public void StopSFX(SFXType type)
    {
        AudioClip clip = GetSFXClip(type);
        if (clip == null) return;

        foreach (AudioSource source in GetComponents<AudioSource>())
        {
            if (source.clip == clip)
            {
                source.Stop();
                Destroy(source);
            }
        }
    }

    private void PlayLoopingSFX(SFXType type)
    {
        if (activeLoopingSFX.Contains(type)) return;

        AudioClip clip = GetSFXClip(type);
        if (clip == null) return;

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = clip;
        newSource.loop = true;
        newSource.volume = sfxVolume;
        newSource.outputAudioMixerGroup = _sfxSource.outputAudioMixerGroup;
        newSource.Play();

        loopingSources.Add(newSource);
        activeLoopingSFX.Add(type);
    }

    public void StopLoopingSFX(SFXType type)
    {
        AudioClip clip = GetSFXClip(type);
        if (clip == null) return;

        for (int i = loopingSources.Count - 1; i >= 0; i--)
        {
            if (loopingSources[i].clip == clip)
            {
                loopingSources[i].Stop();
                Destroy(loopingSources[i]);
                loopingSources.RemoveAt(i);
                activeLoopingSFX.Remove(type);
                break;
            }
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    private AudioClip GetSFXClip(SFXType type)
    {
        foreach (var sfx in sfxClips)
        {
            if (sfx.type == type)
                return sfx.clip;
        }
        return null;
    }
}
